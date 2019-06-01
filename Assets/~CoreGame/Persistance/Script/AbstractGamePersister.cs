using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace CoreGame
{
    public abstract class AbstractGamePersister<T>
    {
        #region External Dependencies
        private PersistanceManager PersistanceManager;
        #endregion

        private string persisterFolderName;
        private string fileExtension;
        private string fileName;

        private BinaryFormatter binaryFormatter = new BinaryFormatter();

        private string folderPath;

        protected AbstractGamePersister(string persisterFolderName, string fileExtension, string fileName)
        {
            this.persisterFolderName = persisterFolderName;
            this.fileExtension = fileExtension;
            this.fileName = fileName;
            folderPath = Path.Combine(Application.persistentDataPath, persisterFolderName);
            this.PersistanceManager = GameObject.FindObjectOfType<PersistanceManager>();
        }


        public T Load()
        {
            if (Directory.Exists(folderPath))
            {
                var path = this.GetDataPath();
                var directoryFiles = Directory.GetFiles(folderPath, this.fileName + this.fileExtension);
                if (directoryFiles.Length > 0)
                {
                    using (FileStream fileStream = File.Open(path, FileMode.Open))
                    {
                        Debug.Log(MyLog.Format("Loaded : " + path));
                        return (T)binaryFormatter.Deserialize(fileStream);
                    }
                }
            }

            return default(T);
        }

        public void SaveAsync(T dataToSave)
        {
            this.PersistanceManager.OnPersistRequested(() =>
            {
                this.SaveSync(dataToSave);
            });
        }

        public bool SaveSync(T dataToSave)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);


            using (FileStream fileStream = File.Open(GetDataPath(), FileMode.OpenOrCreate))
            {
                binaryFormatter.Serialize(fileStream, dataToSave);
                return true;
            }
        }

        public string GetDataPath()
        {
            return Path.Combine(this.folderPath, this.fileName + this.fileExtension);
        }
    }
}
