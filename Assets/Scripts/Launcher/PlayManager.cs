using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Threading;
using System.Windows;
//using ICSharpCode.SharpZipLib.Zip;
//using ICSharpCode.SharpZipLib.Core;

//using System.Windows.Controls;

public class PlayManager : MonoBehaviour
{
    private string rootPath;
    private string versionFile;
    private string gameZip;
    private string gameExe;
    public bool isbusy = false;

    public static PlayManager instance;

    private void Awake()
    {
        instance = this;
    }
    public void PlayGame()
    {

        if (File.Exists(gameExe))
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
            startInfo.WorkingDirectory = Path.Combine(rootPath, "Build");
            Process.Start(startInfo);

            Application.Quit();
        }
        else
        {
            CheckForUpdates();
        }

        

    }

    private void Start()
    {
        rootPath = Directory.GetCurrentDirectory();
        versionFile = Path.Combine(rootPath, "Version.txt");
        gameZip = Path.Combine(rootPath, "Build.zip");
        gameExe = Path.Combine(rootPath, "Build", "Ruffy World.exe");
        progressBar.enabled = false;
        if (File.Exists(versionFile))
        {
            Version localVersion = new Version(File.ReadAllText(versionFile));
            VersionText.text = localVersion.ToString();
        }
        SizeText.text = "";
        CheckForUpdates();
        //PlayGame();
    }
    public Image progressBar;
    public Image progressBar2;
    public Text label;
    public Text label2;
    public Text VersionText;
    public Text SizeText;

    public EntryEffect updateobject;
    public EntryEffect downloadobject;
    //public Button playButton;

    private void CheckForUpdates()
    {
        isbusy = false;
        if (File.Exists(versionFile))
        {
            Version localVersion = new Version(File.ReadAllText(versionFile));
            VersionText.text = localVersion.ToString();

            try
            {
                WebClient webClient = new WebClient();
                Version onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1IqnqmR3o2HshpLEfn1M71RwMzxXq7A3E"));

                if (onlineVersion.IsDifferentThan(localVersion))
                {
                    updateobject.Open();
                    InstallGameFiles(true, onlineVersion);
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        else
        {
            VersionText.text = "1.0.0";
            downloadobject.Open();
            InstallGameFiles(false, Version.zero);
        }
    }

    private void InstallGameFiles(bool _isUpdate, Version _onlineVersion)
    {
        isbusy = true;
        try
        {
            WebClient webClient = new WebClient();
            if (_isUpdate)
            {
                
            }
            else
            {
                
                _onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1IqnqmR3o2HshpLEfn1M71RwMzxXq7A3E"));
            }

            progressBar.enabled = true;
            progressBar2.enabled = true;
            //PlayButton.IsEnabled = false;

            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
            webClient.DownloadFileAsync(new Uri("https://www.googleapis.com/drive/v3/files/1lNWcGCv-f7Y9zIHpvzQeQjVDZdjcVIzu?alt=media&key=AIzaSyCwlQG8vUgxStsFcPuGB-iCfmPpuKqOWLM"), gameZip, _onlineVersion);
        }
        catch (Exception ex)
        {
            
        }
    }

    void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        SizeText.text = (e.TotalBytesToReceive / 1024 / 1024).ToString() + "MB";
        double bytesIn = double.Parse(e.BytesReceived.ToString());
        double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
        double percentage = bytesIn / totalBytes * 100;
        label.text =(e.BytesReceived / (e.TotalBytesToReceive / 100.0f)).ToString("0.0") + " %";
        label2.text = (e.BytesReceived / (e.TotalBytesToReceive / 100.0f)).ToString("0.0") + " %";
        progressBar.fillAmount = (float)(percentage / 100);
        progressBar2.fillAmount = (float)(percentage / 100);
    }

    private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        isbusy = false;
        try
        {
            progressBar.enabled = false;
            progressBar2.enabled = false;
            label.text = "";
            label2.text = "";
            string onlineVersion = ((Version)e.UserState).ToString();
            VersionText.text = onlineVersion;
            //LoginGuiManagerLauncher.instance.ErrorMasage("Entered", Color.red);
            VersionText.text = onlineVersion;
            //LoginGuiManagerLauncher.instance.ErrorMasage(rootPath, Color.red);
            ExtractZipContent(gameZip, null, rootPath);
            File.Delete(gameZip);
            
            File.WriteAllText(versionFile, onlineVersion);
            updateobject.Close();
            downloadobject.Close();
        }
        catch (Exception ex)
        {
            LoginGuiManagerLauncher.instance.ErrorMasage(ex.Message, Color.red);
        }

        //PlayButton.IsEnabled = true;
    }

    public void ExtractZipContent(string FileZipPath, string password, string OutputFolder)
    {
        /*ICSharpCode.SharpZipLib.Zip.ZipFile file = null;
        try
        {
            FileStream fs = File.OpenRead(FileZipPath);
            file = new ICSharpCode.SharpZipLib.Zip.ZipFile(fs);

            if (!String.IsNullOrEmpty(password))
            {
                // AES encrypted entries are handled automatically
                file.Password = password;
            }

            foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry in file)
            {
                if (!zipEntry.IsFile)
                {
                    // Ignore directories
                    continue;
                }

                String entryFileName = zipEntry.Name;
                // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                // Optionally match entrynames against a selection list here to skip as desired.
                // The unpacked length is available in the zipEntry.Size property.

                // 4K is optimum
                byte[] buffer = new byte[4096];
                Stream zipStream = file.GetInputStream(zipEntry);

                // Manipulate the output filename here as desired.
                String fullZipToPath = Path.Combine(OutputFolder, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);

                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }

                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                // of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
            }
        }
        finally
        {
            if (file != null)
            {
                file.IsStreamOwner = true; // Makes close also shut the underlying stream
                file.Close(); // Ensure we release resources
            }
        }*/
    }
    struct Version
    {
        internal static Version zero = new Version(0, 0, 0);

        private short major;
        private short minor;
        private short subMinor;

        internal Version(short _major, short _minor, short _subMinor)
        {
            major = _major;
            minor = _minor;
            subMinor = _subMinor;
        }
        internal Version(string _version)
        {
            string[] versionStrings = _version.Split('.');
            if (versionStrings.Length != 3)
            {
                major = 0;
                minor = 0;
                subMinor = 0;
                return;
            }

            major = short.Parse(versionStrings[0]);
            minor = short.Parse(versionStrings[1]);
            subMinor = short.Parse(versionStrings[2]);
        }

        internal bool IsDifferentThan(Version _otherVersion)
        {
            if (major != _otherVersion.major)
            {
                return true;
            }
            else
            {
                if (minor != _otherVersion.minor)
                {
                    return true;
                }
                else
                {
                    if (subMinor != _otherVersion.subMinor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"{major}.{minor}.{subMinor}";
        }
    }


}
