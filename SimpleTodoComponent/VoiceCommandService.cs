using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using SimpleTodoComponent.DataObjects;

namespace SimpleTodoComponent
{
    public sealed class VoiceCommandService : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        VoiceCommandServiceConnection _voiceServiceConnection;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (triggerDetails?.Name == "SimpleTodoVoiceCommandService")
            {
                _deferral = taskInstance.GetDeferral();
                var cancelledTokenSource = new CancellationTokenSource();
                _voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                var voiceCommand = await _voiceServiceConnection.GetVoiceCommandAsync();

                switch (voiceCommand.CommandName)
                {
                    case "todoBackgroundCommandList":
                        var command = voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties["command"][0];
                        if (command == "pending")
                            await ShowPendingTasks();
                        break;

                    default:
                        LaunchAppInForeground();
                        break;
                }

                taskInstance.Canceled += (s, e) =>
                {
                    cancelledTokenSource.Cancel();
                    _deferral.Complete();
                };

                _voiceServiceConnection.VoiceCommandCompleted += (sender, args) =>
                {
                    cancelledTokenSource.Cancel();
                    _deferral.Complete();
                };

                _voiceServiceConnection.VoiceCommandCompleted += (sender, args) =>
                {
                    cancelledTokenSource.Cancel();
                };
            }
        }

        private async Task ShowPendingTasks()
        {

            VoiceCommandUserMessage userMessage;
            var todoContentTiles = new List<VoiceCommandContentTile>();

            try
            {
                var todos = await Utility.ReadTodosFromLocalFolderAsync();
                var todoList = todos.ToList();

                if (todoList?.Count == 0)
                {
                    userMessage = new VoiceCommandUserMessage
                    {
                        DisplayMessage = "There is no task in your to do list.",
                        SpokenMessage = "There is no task in your to do list."
                    };

                    var response = VoiceCommandResponse.CreateResponse(userMessage, todoContentTiles);
                    await _voiceServiceConnection.ReportSuccessAsync(response);
                }
                else
                {
                    var spokenMessage = string.Empty;

                    if (todoList != null)
                        foreach (var todo in todoList)
                        {
                            if (todo.IsDone) continue;
                            var todoContentTile = new VoiceCommandContentTile
                            {
                                ContentTileType = VoiceCommandContentTileType.TitleOnly,
                                Title = todo.Title
                            };

                            spokenMessage += todo.Title;

                            todoContentTiles.Add(todoContentTile);
                        }

                    if (!todoList.Any())
                    {
                        userMessage = new VoiceCommandUserMessage
                        {
                            DisplayMessage = "Here’s your pending task.",
                            SpokenMessage = spokenMessage
                        };
                    }
                    else
                    {
                        userMessage = new VoiceCommandUserMessage
                        {
                            DisplayMessage = "Here are your pending tasks.",
                            SpokenMessage = spokenMessage
                        };
                    }
                    var response = VoiceCommandResponse.CreateResponse(userMessage, todoContentTiles);
                    await _voiceServiceConnection.ReportSuccessAsync(response);
                }
            }
            catch (Exception)
            {
                userMessage = new VoiceCommandUserMessage
                {
                    DisplayMessage = "There is no task in your to do list.",
                    SpokenMessage = "There is no task in your to do list."
                };
                var response = VoiceCommandResponse.CreateResponse(userMessage, todoContentTiles);
                await _voiceServiceConnection.ReportSuccessAsync(response);
            }
        }

        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage { SpokenMessage = "Launching My Simple Todo" };

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            // When launching the app in the foreground, pass an app 
            // specific launch parameter to indicate what page to show.
            //response.AppLaunchArgument = "showAllTrips=true";

            await _voiceServiceConnection.RequestAppLaunchAsync(response);
        }
    }
}
