var MyPlugin = {
    Hello: function(chanel,uid,volume)
    {
        window.alert("Hello, world!");
    },
    closewindow: function ()
    {
           window.alert("Leave This Page.");
           window.close();
    },
    OpenURLInExternalWindow: function (url) {
     window.open(Pointer_stringify(url), "_blank");
	},
    AgoraCallBack: function(channelName,uid,elapsed)
    {
        window.alert(uid);
    },
    AddNumbers: function(x,y)
    {
        return x + y;
    },
    StringReturnValueFunction: function()
    {
        var returnStr = "bla";
        var buffer = _malloc(lengthBytesUTF8(returnStr) + 1);
        writeStringToMemory(returnStr, buffer);
        return buffer;
    },
    BindWebGLTexture: function(texture)
    {
        GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
    },
    GoFullscreen: function()
    {
        var viewFullScreen = document.getElementById('myCanvas');
        var ActivateFullscreen = function()
        {
            if (viewFullScreen.requestFullscreen) /* API spec */
            {  
                viewFullScreen.requestFullscreen();
            }
            else if (viewFullScreen.mozRequestFullScreen) /* Firefox */
            {
                viewFullScreen.mozRequestFullScreen();
            }
            else if (viewFullScreen.webkitRequestFullscreen) /* Chrome, Safari and Opera */
            {  
                viewFullScreen.webkitRequestFullscreen();
            }
            else if (viewFullScreen.msRequestFullscreen) /* IE/Edge */
            {  
                viewFullScreen.msRequestFullscreen();
            }
 
            viewFullScreen.removeEventListener('touchend', ActivateFullscreen);
        }
 
        viewFullScreen.addEventListener('click', ActivateFullscreen, true);
        
    }
};

mergeInto(LibraryManager.library, MyPlugin);