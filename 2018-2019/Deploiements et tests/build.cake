using Cake.Common.Tools;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.IO;
using System.Diagnostics;
using System.IO;
#addin "Cake.FileHelpers"

string adbPath = @"C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe";


var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var outputApkFullDirectoryPath = Argument("OutputApkFullDirectoryPath", @"\\monnas\Installers\Soft\Android\FarmeosSmartScan");

var rootDir = Directory(".");
var buildDir = rootDir+Directory("build");
var buildOutputDir = rootDir+Directory("build-output");
EnsureDirectoryExists(buildDir);
EnsureDirectoryExists(buildOutputDir);

var srcDir = rootDir + Directory("src");

var mobileAndroidDir = srcDir + Directory("Mobile.Android");
var mobileAndroidCSProj=mobileAndroidDir + File("Mobile.Android.csproj");
var mobileDir = srcDir + Directory("Mobile");
//nom du fichier buildé par BuildAPK
	var apkFileName = File("fr.monappli.mobile-Signed.apk");

string[] oldStyleProjects = new string[]{
	mobileAndroidCSProj
};

string[] newStyleProjects = new string[]{
	mobileDir+File("Mobile.csproj")
};

Task("CleanOldStyleProjects")
.Description("Nettoyage de tous les projets encore sur l'environnement MSBuild classique")
.DoesForEach(oldStyleProjects,(project)=>{
	DotNetBuild(project, settings=>{
		settings.SetConfiguration(configuration)
			.WithTarget("Clean");
	});
});


Task("CleanNewStyleProjects")
.Description("Nettoyage de tous les projets .Net Core fonctionnant avec dotnet.exe")
.DoesForEach(newStyleProjects,(project)=>{
	DotNetBuild(project, settings=>{
		settings.SetConfiguration(configuration)
			.WithTarget("Clean");
	});
});

Task("Clean")
	.Description("Nettoyage de tous les projets")
	.IsDependentOn("CleanOldStyleProjects")
	.IsDependentOn("CleanNewStyleProjects");

Task("RestoreNewStyleProjects")
.Description("Restauration des dépendances de tous les projets .Net Core fonctionnant avec dotnet.exe")
.DoesForEach(newStyleProjects,(project)=>{
	DotNetCoreRestore(project, new DotNetCoreRestoreSettings(){
		PackagesDirectory = "Packages",
		NoCache = true
	});
});

Task("RestoreOldStyleProjects")
.Description("Restauration des dépendances de tous les projets encore sur l'environnement MSBuild classique")
.DoesForEach(oldStyleProjects,(project)=>{
	NuGetRestore(project, new NuGetRestoreSettings {
		PackagesDirectory = "Packages",
		NoCache = true 
	});
});

Task("Restore")
	.Description("Restauration des dépendances de tous les projets")
	.IsDependentOn("RestoreNewStyleProjects")
	.IsDependentOn("RestoreOldStyleProjects");

FilePath manifestFilePath=null;
String lastApkFileNameVersionSuffix=null;

Task("BuildAPK")
	.Description("Mobile.Android APK Building")
	.IsDependentOn("Restore")
	.IsDependentOn("TransformManifest")
	.Does(()=>{
	var manifestRelativePath = MakeAbsolute(mobileAndroidDir).GetRelativePath(MakeAbsolute(manifestFilePath)).FullPath;
	Information("Utilisation du manifest "+manifestRelativePath);
	DotNetBuild(mobileAndroidCSProj, settings=>{
		settings.SetConfiguration(configuration)
			.WithProperty("OutputPath", MakeAbsolute(buildDir).FullPath+"/")
			.WithProperty("AndroidManifest",manifestRelativePath)
			.WithTarget("SignAndroidPackage");
	});
	CopyFiles(buildDir.Path.FullPath + @"\*.apk", buildOutputDir);
});

Task("TransformManifest")
	.Does(()=>{

	var manifestOriginal = mobileAndroidDir + Directory("Properties") + File("AndroidManifest.xml");
	manifestFilePath = buildDir + File("AndroidManifest-transformed.xml");
	CopyFile(manifestOriginal,manifestFilePath);

	var xmlNamespaces= new Dictionary<string, string> 
		{
			{"android", "http://schemas.android.com/apk/res/android"}
		};

	var versionName = XmlPeek(manifestFilePath,"/manifest/@android:versionName",new XmlPeekSettings {
        Namespaces = xmlNamespaces
    }) ;

	int versionCode = Int32.Parse(XmlPeek(manifestFilePath,"/manifest/@android:versionCode",new XmlPeekSettings {
        Namespaces = xmlNamespaces
    }));
	
	lastApkFileNameVersionSuffix=String.Format("{0:yyMMddHHmmss}-{1}", DateTime.Now, Environment.MachineName);
	XmlPoke(manifestFilePath, "/manifest/@android:versionName",String.Format("{0}-{1}-{2}", versionName,configuration, lastApkFileNameVersionSuffix), new XmlPokeSettings
	{
		Namespaces = xmlNamespaces
	});
	FileWriteText(buildOutputDir + apkFileName + "_infos.txt", lastApkFileNameVersionSuffix);
	Int64 newVersionCode=(Int64.Parse(String.Format("{0:yyMMddHHmm}", DateTime.Now))-1800000000); //Max allowed 2100000000
	XmlPoke(manifestFilePath, "/manifest/@android:versionCode",newVersionCode.ToString(), new XmlPokeSettings
	{
		Namespaces = xmlNamespaces
	});
	
});


Task("PublishAPK")
	.Description("Mobile.Android APK publication vers Nasdev")
	.IsDependentOn("Clean")
	.IsDependentOn("BuildAPK")
	.Does(()=>{
	
	//dossier du fichier buildé par BuildAPK
	var apkFilePath = buildOutputDir + apkFileName;

	var destinationFolder = Directory(outputApkFullDirectoryPath);
	string destinationFile = destinationFolder + apkFileName;

	CopyFile(apkFilePath,destinationFile);
	CopyFile(apkFilePath+"_infos.txt",destinationFile+"_infos.txt");

});


Task("InstallAPK")
	.Description("Mobile.Android APK - installation sur le device physique connecté. Le device doit être appelable via ADB. Seul un device connecté géré pour le moment")
	.IsDependentOn("Restore")
	.IsDependentOn("TransformManifest")
	.Does(()=>{
		var manifestRelativePath = MakeAbsolute(mobileAndroidDir).GetRelativePath(MakeAbsolute(manifestFilePath)).FullPath;
		DotNetBuild(mobileAndroidCSProj, settings=>{
			settings.SetConfiguration(configuration)
				//Connection sur le device physique connecté. Si emulateur, il faudrait passer cela à "-e"
				//cf. https://developer.android.com/studio/command-line/adb.html#issuingcommands
				.WithProperty("AdbTarget","-d")
				.WithProperty("OutputPath", MakeAbsolute(buildDir).FullPath+"/")
				.WithProperty("AndroidManifest",manifestRelativePath)
				.WithTarget("Install");
		});
	});

int executeProcessAndGetOutput(string processName, string processArguments, out string output){
	IEnumerable<string> redirectedStandardOutput;
	var returnCode = StartProcess(
         processName,
         new ProcessSettings {
             Arguments = processArguments,
             RedirectStandardOutput = true
         },
         out redirectedStandardOutput
     );
	 output= String.Join(Environment.NewLine,redirectedStandardOutput.Select(l=>l.Trim()));
	 return returnCode;
}

Task("StartAPK")
	.Description("Mobile.Android APK - installation sur le device physique connecté puis démarrage de l'activité. Le device doit être appelable via ADB. Seul un device connecté géré pour le moment")
	.IsDependentOn("InstallAPK")
	.Does(()=>{
		int returnCode = executeProcessAndGetOutput(adbPath, "shell input keyevent KEYCODE_WAKEUP", out string output);
		Information(output);
		if(returnCode != 0){
			throw new Exception($"Erreur lors du réveil du smartscan. Code de retour de ADB : {returnCode}");
		}

		returnCode =executeProcessAndGetOutput(adbPath, "shell monkey -p fr.monappli.mobile -c android.intent.category.LAUNCHER 1", out output);
		Information(output);

		if(returnCode != 0){
			throw new Exception($"Erreur lors du lancement du smartscan. Code de retour de ADB : {returnCode}");
		}
});



Task("UninstallAPK")
	.Description("Mobile.Android APK - désinstallation du device physique connecté. Le device doit être appelable via ADB. Seul un device connecté géré pour le moment")
	.Does(()=>{
		DotNetBuild(mobileAndroidCSProj, settings=>{
			settings.SetConfiguration(configuration)
				//Connection sur le device physique connecté. Si emulateur, il faudrait passer cela à "-e"
				//cf. https://developer.android.com/studio/command-line/adb.html#issuingcommands
				.WithProperty("AdbTarget","-d")
				.WithProperty("OutputPath", MakeAbsolute(buildDir).FullPath+"/")
				.WithTarget("Uninstall");
		});
	});



Task("ReinstallAPK")
	.Description("Mobile.Android APK - réinstallation complète du device physique connecté. Le device doit être appelable via ADB. Seul un device connecté géré pour le moment")
	.IsDependentOn("UninstallAPK")
	.IsDependentOn("InstallAPK");

Task("Build")
.Description("Build de tous les projets de la solution")
.Does(() =>{
	DotNetBuild("./Smartscan.sln", settings=>{
		settings.SetConfiguration(configuration)
			.WithProperty("OutputPath", MakeAbsolute(buildDir).FullPath+"/")
			.WithTarget("Build");
	});
});

Task("FullBuild")
	.Description("Build complète : nettoyage, restauration des dépendances et build des projets de la solution")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Build");


Task("InstallEHS")
	.Does(()=>{
		int returnCode= executeProcessAndGetOutput(adbPath, @"install -r "".\Deploiement\Applications\EHS_020703_A.apk""", out string output);
		Information(output);
		if(returnCode != 0){
			throw new Exception($"Erreur lors du lancement du smartscan. Code de retour de ADB : {returnCode}");
		}
});

Task("InstallAppGallery")
	.Does(()=>{
		int returnCode = executeProcessAndGetOutput(adbPath, @"install -r "".\Deploiement\Applications\appgallery-v3.0.1.4.apk""", out string output);
		Information(output);
		if(returnCode != 0){
			throw new Exception($"Erreur lors du lancement du smartscan. Code de retour de ADB : {returnCode}");
		}
});

Task("InstallEHSConfig")
	.Does(()=>{
		int returnCode = executeProcessAndGetOutput(adbPath, @"push "".\Deploiement\Configurations\enterprisehomescreen.xml"" /enterprise/usr", out string output);
		Information(output);
		if(returnCode != 0){
			throw new Exception($"Erreur lors du lancement du smartscan. Code de retour de ADB : {returnCode}");
		}
});


Task("ConfigureTC")
	.IsDependentOn("InstallEHSConfig")
	.IsDependentOn("InstallAppGallery");

Task("DestroyLocalDotnetCaches")
.Does(() => {
	int returnCode = executeProcessAndGetOutput("dotnet.exe", @"nuget locals all -c", out string output);
	Information(output);
	if(returnCode != 0){
		throw new Exception($"Erreur lors du lancement du smartscan. Code de retour de ADB : {returnCode}");
	}
});


RunTarget(target);
