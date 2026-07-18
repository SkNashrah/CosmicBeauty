using CommunityToolkit.Mvvm.Input;
using CosmicBeauty.Models;

namespace CosmicBeauty.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}