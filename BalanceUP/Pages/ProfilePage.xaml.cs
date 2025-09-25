using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using System;
using System.IO;
using System.Threading.Tasks;
using BalanceUP.Data;
using BalanceUP.Models;

namespace BalanceUP.Pages
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadProfile(); // Load the saved profile data when the page is initialized
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // Reset the current user on logout
            UserInfo.Current = null; // Ensure the current user is reset

            // Navigate back to the LoginPage after clicking logout
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }

        private async void OnUsernameTapped(object sender, EventArgs e)
        {
            // Change username
            string newUsername = await DisplayPromptAsync("Change Username", "Enter your new username:", initialValue: UsernameLabel.Text);
            if (!string.IsNullOrWhiteSpace(newUsername))
            {
                UsernameLabel.Text = newUsername;
                // Save the new username to the database
                DatabaseHelper databaseHelper = new DatabaseHelper();
                var currentUser = UserInfo.Current; // Get the current user
                if (currentUser != null)
                {
                    currentUser.Username = newUsername; // Update the username
                    databaseHelper.UpdateUser(currentUser); // Save the updated user to the database
                }
            }
        }

        private async Task<bool> CheckAndRequestCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission Denied", "Camera permission is needed to take photos", "OK");
                return false;
            }

            return true;
        }

        private async Task<bool> CheckAndRequestStoragePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            var writeStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (writeStatus != PermissionStatus.Granted)
            {
                writeStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            if (status != PermissionStatus.Granted || writeStatus != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission Denied", "Storage permission is needed to save photos", "OK");
                return false;
            }

            return true;
        }

        private async void OnCameraClicked(object sender, EventArgs e)
        {
            try
            {
                // Check for camera and storage permissions first
                bool cameraPermission = await CheckAndRequestCameraPermission();
                bool storagePermission = await CheckAndRequestStoragePermission();

                if (!cameraPermission || !storagePermission)
                    return;

                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo != null)
                {
                    // Copy the photo to a local file
                    var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                    using (var sourceStream = await photo.OpenReadAsync())
                    using (var destinationStream = File.Create(localFilePath))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }

                    // Use the local file path instead
                    ProfileImage.Source = ImageSource.FromFile(localFilePath);

                    // Save the image path to the database
                    DatabaseHelper databaseHelper = new DatabaseHelper();
                    var currentUser = UserInfo.Current; // Get the current user
                    if (currentUser != null)
                    {
                        currentUser.ProfileImagePath = localFilePath; // Update the profile image path
                        databaseHelper.UpdateUser(currentUser); // Save the updated user to the database
                    }
                }
            }
            catch (Exception ex)
            {
                string errorDetails = $"Type: {ex.GetType().Name}\nMessage: {ex.Message}\nStack Trace: {ex.StackTrace}";
                if (ex.InnerException != null)
                {
                    errorDetails += $"\nInner Exception: {ex.InnerException.Message}";
                }

                await DisplayAlert("Detailed Error", errorDetails, "OK");
                Console.WriteLine(errorDetails);
            }
        }

        private async void OnGalleryClicked(object sender, EventArgs e)
        {
            try
            {
                // Check for storage permissions first
                bool storagePermission = await CheckAndRequestStoragePermission();

                if (!storagePermission)
                    return;

                var photo = await MediaPicker.PickPhotoAsync();
                if (photo != null)
                {
                    // Copy the photo to a local file
                    var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                    using (var sourceStream = await photo.OpenReadAsync())
                    using (var destinationStream = File.Create(localFilePath))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }

                    // Use the local file path instead
                    ProfileImage.Source = ImageSource.FromFile(localFilePath);

                    // Save the image path to the database
                    DatabaseHelper databaseHelper = new DatabaseHelper();
                    var currentUser = UserInfo.Current;
                    if (currentUser != null)
                    {
                        currentUser.ProfileImagePath = localFilePath; // Update the profile image path
                        databaseHelper.UpdateUser(currentUser); // Save the updated user to the database
                    }
                }
            }
            catch (Exception ex)
            {
                string errorDetails = $"Type: {ex.GetType().Name}\nMessage: {ex.Message}\nStack Trace: {ex.StackTrace}";
                if (ex.InnerException != null)
                {
                    errorDetails += $"\nInner Exception: {ex.InnerException.Message}";
                }

                await DisplayAlert("Detailed Error", errorDetails, "OK");
                Console.WriteLine(errorDetails);
            }
        }

        private void LoadProfile()
        {
            // Load the current user's profile data
            var currentUser = UserInfo.Current;
            if (currentUser != null)
            {
                UsernameLabel.Text = currentUser.Username; // Set username label
                EmailLabel.Text = currentUser.Email; // Set email label

                // Check if the profile image path exists
                if (!string.IsNullOrEmpty(currentUser.ProfileImagePath) && File.Exists(currentUser.ProfileImagePath))
                {
                    ProfileImage.Source = ImageSource.FromFile(currentUser.ProfileImagePath);
                }
                else
                {
                    ProfileImage.Source = "profile.svg"; // Default image
                }
            }
            else
            {
                UsernameLabel.Text = "Default Username"; // Default value if no user is logged in
                EmailLabel.Text = "N/A"; // Default value for email
                ProfileImage.Source = "profile.svg"; // Default image
            }
        }

        private async void OnProfileImageTapped(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Change Profile Picture", "Cancel", null, "Camera", "Gallery");

            switch (action)
            {
                case "Camera":
                    OnCameraClicked(sender, e);
                    break;
                case "Gallery":
                    OnGalleryClicked(sender, e);
                    break;
                case "Cancel":
                    // Do nth
                    break;
            }
        }

        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChangePasswordPage());
        }
    }
}