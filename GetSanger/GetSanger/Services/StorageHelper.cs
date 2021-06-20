using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace GetSanger.Services
{
    public class StorageHelper : Service
    {
        private readonly StorageClient r_StorageClient;
        private readonly string r_BucketName = "get-sanger.appspot.com";

        public StorageHelper()
        {
            try
            {
                var googleCredential = GoogleCredential.FromJson(
                    "{\n  \"type\": \"service_account\",\n  \"project_id\": \"get-sanger\",\n  \"private_key_id\": \"755180025657c28a6473e3cdd46b0af54e28bbd1\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQC4FPRcgXUwEfr9\\ngf2Z8DTu652C5ET84hiyS74Qu2YK636dsPwT/LikmtCRVjIlmW3nylAATVi4LFoM\\n7f5O3mGD5tUf8pbHQjS69Ja+J/xDmw2CBGDBFz3FZIWN2aUrQNAc6PumFuiS9Imf\\nwXYI+etuOzVtCM8Q973w6exKPDj/qQ7wgLLs34TxwxNlwlPngIt282pM69UynXiW\\noXOOKp/STYfVs0gom6No5gvtaCJv3amoyBVg4P7x5w2QonStHNcTCULAJh0+x1fF\\n3J8d9GY6eOwm10tRj7rmR/6FI7DwRUOu7Ui2C7pwfxGjrKzg/5FcRLM8cVBJYCxS\\ncVaSG0I/AgMBAAECggEABtypG0WsI6WX8JlG5dUhdAKOdZGizLZ06ziNljwWGrQa\\nnTA6xVf3XhNzUOegqOwmP6y3/LrlYsAsfyzj/PanNe6F5VH6suTR6GIzuFTvP2np\\nP/5/yNLf95q+q8IAOTOKTnmU3+UcuFWrR6xDhSQj1ZEZdb4NUfmaN//QUhgam9UW\\nSnsoWYXv60XPPps2nTa6iLKaMlNi/6V9Uj9iX21W5YXZxr0AXrfcvytf+/oiB7z/\\nhGPfhjpmXaXZ1/n8oEEOraSrPysvG2dUfEEjV4dYfn+35sO79E+YExNMbza8tEa8\\nPMWOH2bw5EaP2M5U/l6dPYmeGHZzLIEyuJNy31h3UQKBgQDhKpZc0epcMa+E0HoS\\nR63Y9Vl6SLvhfYpML+eM/sfaIJhPxLuNkWs1ROLRLTJx2cAJPW75FSOwQoN7qNPE\\nRBCviF0OqVSonfFOKnLpqSbRpGb+S/MLY5D36eFVUXDHTD387e2h4Tt9xrX2CRGg\\naKI/o9Ycg8BYsIQe2OuMa0PTKQKBgQDRShzB94JDjekq/WUM3t7GREkNJ8En44zW\\nSsc/ppML0AS5AAPfgXk1wsFn4MlxGP6VD7iv7ud5pMhTXUs4ZsZ6CssguFukxQwe\\n0Ws32VQgmdH3PEdo5cOew3Ydx1QxGwXcPCXOV1/EcoOeqBrOx9X48rWsnS3p+rGW\\n4IM6TDc/JwKBgQClQgCBt39ukvjjXa96YylxJwG5AlECuoUaTb6AeS5tJackRJvy\\n11v8CMcAB1ASx2JuZaxhG+bJoselps9mKR6l9kt2S0lbSDCm0fjtDbzz+NmuokeL\\ngFQR4JIFcHR5kbedP3M9cHWdZo5+OUG6nZEXbEPlQb2WWldd3I9kzAVHoQKBgQC5\\nF7pnMOZbqF3WT1PXFiQfZLTWlZoWFQCxZGDiwga6I7aJhqHmQTNZgGxb2klNZLWC\\nTu0fa0H8KuegEpo7v2k49eK/hUxHZJ1bhAo20lW8n006Qm1FMC7ZcjQ8EdU7uJJu\\nPCOTYWKo8j3FSxaqWcG68awKhNfeXoBA1z43iMAzcQKBgQCM6jl8Bb1ApLDuRGe7\\nJA+9n0rfmCruAZs3X9Vy3AdwqqUSRal3syJAjngxNLJiC0WQ12C1sXz6beinzf7s\\noxLDLTgeBAmGpUW91CfWNEbLClMmh9vC9CmHXJqJtfZWwZEaMn1JuSkVBaTIPce1\\nJvWMs7dHYACkOi+KHsvfbCQPWQ==\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"get-sanger@appspot.gserviceaccount.com\",\n  \"client_id\": \"115148895941710652799\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/get-sanger%40appspot.gserviceaccount.com\"\n}\n");
                r_StorageClient = StorageClient.Create(googleCredential);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task SetUserProfileImage(User i_User, Stream i_Stream)
        {
            if (i_User.UserId == null)
            {
                throw new ArgumentNullException("User must have an ID");
            }

            if(i_User.ProfilePictureUri != null)
            {
                await DeleteProfileImage(i_User.UserId);
                i_User.ProfilePictureUri = null;
            }

            await UploadFile(i_Stream, $"ProfilePictures/{i_User.UserId}.png");
            i_User.ProfilePictureUri = new Uri($"https://europe-west3-get-sanger.cloudfunctions.net/GetUserProfilePicture?UserId={i_User.UserId}");
        }

        public async Task<Uri> UploadFile(Stream stream, string objectPath)
        {
            stream.Position = 0;
            Object uploadedObject =
                await r_StorageClient.UploadObjectAsync(r_BucketName, objectPath, "image/png", stream);
            uploadedObject.Acl ??= new List<ObjectAccessControl>();
            await r_StorageClient.UpdateObjectAsync(uploadedObject, new UpdateObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            });
            var uploadedObjectMediaLink = uploadedObject.MediaLink;

            var uri = new Uri(uploadedObjectMediaLink);

            return uri;
        }

        public async Task DeleteProfileImage(string i_UserID)
        {
            await DeleteFile($"ProfilePictures/{i_UserID}.png");
        }

        public async Task DeleteFile(string objectPath)
        {
            try
            {
                await r_StorageClient.DeleteObjectAsync(r_BucketName, objectPath);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public override void SetDependencies()
        {
        }
    }
}