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
            var path = this.GetDataPath();
            return this.PersistanceManager.Load<T>(folderPath, path, this.fileName, this.fileExtension);
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
