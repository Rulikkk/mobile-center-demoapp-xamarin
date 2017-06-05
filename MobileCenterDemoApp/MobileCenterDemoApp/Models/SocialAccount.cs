namespace MobileCenterDemoApp.Models
{
    /// <summary>
    /// General information about user
    /// </summary>
    public class SocialAccount
    {
        /// <summary>
        /// Account username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Account identity
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Account photo image source 
        /// </summary>
        public Xamarin.Forms.ImageSource ImageSource { get; set; }
    }
}
