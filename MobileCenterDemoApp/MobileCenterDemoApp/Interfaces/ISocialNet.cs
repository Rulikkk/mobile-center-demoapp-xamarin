namespace MobileCenterDemoApp.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using MobileCenterDemoApp.Models;

    public interface ISocialNet
    {
        /// <summary>
        /// Try login in social net (Facebook or twitter)
        /// </summary>
        /// <returns>
        /// Social account if success
        ///     null if login failed
        /// </returns>
        Task<SocialAccount> Login();

        /// <summary>
        /// Event raise when get some error
        ///     Send: error message
        /// </summary>
        event Action<string> OnError;
    }

    /// <summary>
    /// Interface for login via Facebook
    /// </summary>
    public interface IFacebook : ISocialNet
    {

    }

    /// <summary>
    /// Interface for login via Twitter
    /// </summary>
    public interface ITwitter : ISocialNet
    {
        
    }
}
