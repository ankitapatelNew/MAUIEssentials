namespace MAUIEssentials.AppCode.Helpers
{
    public class DataManager<T> where T : new()
	{
        public SQLiteConnection dbConn;
		private static volatile DataManager<T> _instance;
		public static object SyncObject = new object();

		private static Type[] DataBaseTypes;

		public static DataManager<T> Instance {
			get {
				if (_instance == null) {
					lock (SyncObject) {
						if (_instance == null) {
							_instance = new DataManager<T>();
						}
					}
				}

				return _instance;
			}
		}

		public DataManager()
		{
			try
			{
                SetConnection();
            }
			catch (Exception ex)
			{
				ex.LogException();
			}
        }

        private void SetConnection()
        {
			try
			{
                var fileName = "SQLiteEx.db3";
                var directory = $"{DependencyService.Get<ICommonUtils>().GetDocumentDirectoryPath()}/datadb";

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var filePath = Path.Combine(directory, fileName);

                if (!File.Exists(filePath))
                {
                    using (File.Create(filePath)) { }
                }

                dbConn = new SQLiteConnection(filePath);
            }
			catch (Exception ex)
			{
                ex.LogException();
            }
        }

        public void CreateDB(Type[] dbTables)
		{
			try {
				// create the table(s)
				DataBaseTypes = dbTables;

				foreach (var tableType in DataBaseTypes) {
					dbConn.CreateTable(tableType);
				}
			} catch (Exception ex) {
				ex.LogException();
			}
		}

		public int Insert(T entity)
		{
			return Instance.dbConn.Insert(entity);
		}

		public int InsertOrReplace(T entity)
		{
			lock (SyncObject) {
				return Instance.dbConn.InsertOrReplace(entity);
			}
		}

		public void InsertAll(List<T> entity)
		{
			lock (SyncObject) {
				try {
					Instance.dbConn.InsertAll(entity);
				} catch (Exception ex) {
					ex.LogException();
				}
			}
		}

		public void InsertOrReplaceAll(List<T> entity)
		{
			lock (SyncObject) {
				foreach (var item in entity) {
					Instance.dbConn.InsertOrReplace(item);
				}
			}
		}

		public void DeleteAll()
		{
			lock (SyncObject) {
				Instance.dbConn.DeleteAll<T>();
			}
		}

		public void DeleteAllData()
		{
			foreach (var tableType in DataBaseTypes) {
				dbConn.Execute("Delete from " + tableType.Name);
			}
		}

		public void DeleteById(string Id)
		{
			lock (SyncObject) {
				Instance.dbConn.Delete<T>(Id);
			}
		}

		public void Delete(T obj)
		{
			lock (SyncObject) {
				Instance.dbConn.Delete<T>(obj);
			}
		}

		public List<T> GetAll()
		{
			try {
				lock (SyncObject) {
					List<T> list = Instance.dbConn.Table<T>().ToList();
					return list;
				}
			} catch (Exception ex) {
				ex.LogException();
			}
			return new List<T>();
		}

		public int Count()
		{
			try {
				lock (SyncObject) {
					return Instance.dbConn.Table<T>().Count();
				}
			} catch (Exception ex) {
				ex.LogException();
			}
			return 0;
		}

		public T Get()
		{
			try {
				lock (SyncObject) {
					T obj = Instance.dbConn.Table<T>().FirstOrDefault();
					return obj;
				}
			} catch (Exception ex) {
				ex.LogException();
			}
			return default;
		}

		public T GetById(string Id)
		{
			try {
				lock (SyncObject) {
					T obj = Instance.dbConn.Get<T>(Id);
					return obj;
				}
			} catch (Exception ex) {
				ex.LogException();
			}
			return default;
		}
	}
}
