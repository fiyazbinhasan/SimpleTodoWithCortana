using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.ApplicationInsights;

namespace UWPCSHARP
{
    sealed partial class App : Application
    {
        public App()
        {
            WindowsAppInitializer.InitializeAsync(
                WindowsCollectors.Metadata |
                WindowsCollectors.Session);
            InitializeComponent();
            Suspending += OnSuspending;
        }
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;
            
            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof (MainPage), e.Arguments);
            }

            Window.Current.Activate();
            await RegisterVoiceCommands();
        }

        private async Task RegisterVoiceCommands()
        {
            var storageFile =
                await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri("ms-appx:///MySimpleTodoCommands.xml"));
            await
                VoiceCommandDefinitionManager
                    .InstallCommandDefinitionsFromStorageFileAsync(storageFile);
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        //protected override void OnActivated(IActivatedEventArgs args)
        //{
        //    string navigationParameterString = string.Empty;
        //    Type navigateToPageType = null;

        //    if (args.Kind != ActivationKind.VoiceCommand)
        //    {
        //        return;
        //    }

        //    var commandArgs = args as VoiceCommandActivatedEventArgs;

        //    if (commandArgs != null)
        //    {
        //        SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;
        //        string voiceCommandName = speechRecognitionResult.RulePath[0];
        //        string textSpoken = speechRecognitionResult.Text;
        //        string commandMode = SemanticInterpretation(speechRecognitionResult);

        //        switch (voiceCommandName)
        //        {
        //            case "todoCommandList":
        //                var command = speechRecognitionResult.SemanticInterpretation.Properties["command"];
        //                if (command.Any(s=>textSpoken.Contains(s)))
        //                {
        //                    navigationParameterString = $"{voiceCommandName}|{commandMode}|{textSpoken}|{command.Single(c=> textSpoken.Contains(c))}";
        //                    navigateToPageType = typeof(MainPage);
        //                }
        //                break;

        //            default:
        //                navigateToPageType = typeof(MainPage);
        //                break;
        //        }
        //    }

        //    Frame rootFrame = Window.Current.Content as Frame;

        //    if (rootFrame != null)
        //        rootFrame.Navigate(navigateToPageType, navigationParameterString);
        //    else
        //        throw new Exception("Failed to create voice command page");
        //}

        private string SemanticInterpretation(SpeechRecognitionResult speechRecognitionResult)
        {
            var semanticProperties = speechRecognitionResult.SemanticInterpretation.Properties;

            if (semanticProperties.ContainsKey("commandMode"))
            {
                return (semanticProperties["commandMode"][0] == "voice" ? CommandModes.Voice : CommandModes.Text).ToString();
            }

            return CommandModes.Text.ToString();
        }

        public enum CommandModes
        {
            Voice,
            Text
        }
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
