namespace PoliceReport.Core.Tools.Updates
{
    public interface IUpdateUserInterface
    {
        void ShowProgressBar(bool visible);
        void UpdateProgressBar(int percentage);
        void EnableUpdateButton(bool enabled);
        void DisplayErrorMessage(Exception ex);
    }
}
