# Pedometer. Demo application (Xamarin). 

This is demo application which shows ablities of Mobile Center.

![](Images/general.png)

Mobile Center brings together multiple services, commonly used by mobile developers, into a single, integrated product. 
Team members can build, test, distribute, and monitor mobile apps. 
The Demo Application intended to represent the features of the Mobile Center API, 
such as managing cross platform applications and collecting statistics about how application behaves on real users’ devices, 
what kind of crashes occurred and so on. Applications were integrated with social services like Twitter and Facebook, and use Google Fit (Health Kit) to show daily data of user’s activity.

## Setting up enviroment for Android
* Install Visual Studio 2017
  * Select toolset **"Mobile development with .Net"**  
* Install Android SDK 
  * Android 7.1.1 (API 25) - SDK Platform
  * Google Play services
  * Google Repository

* Setting up android-sdk for Xamarin
  * Move your android-sdk to forlder withour spaces in path and update path to sdk in Xamarin
  * Download latest version of [Proguard](https://sourceforge.net/projects/proguard/files/proguard/)
  * Replace proguard files in Android-sdk (.\android-sdk\tools\proguard)

## Setting up enviroment for iOS
* Install XCode
* [Install Visual Studio for MAC](https://www.visualstudio.com/vs/visual-studio-mac/)
  * With Xamarin
