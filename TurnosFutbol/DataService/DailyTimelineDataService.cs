using System.Reflection;
using System.Runtime.Serialization.Json;
using TurnosFutbol.ViewModels.Dashboard;
using Xamarin.Forms.Internals;

namespace TurnosFutbol.DataService
{
    /// <summary>
    /// Data service to load the data from json file.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class DailyTimelineDataService
    {
        #region Fields

        private static DailyTimelineDataService instance;
        private DailyTimelineViewModel dailyTimelineViewModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance for the <see cref="DailyTimelineDataService"/> class.
        /// </summary>
        public DailyTimelineDataService()
        {
        }
        #endregion

        #region properties

        /// <summary>
        /// Gets an instance of the <see cref="DailyTimelineDataService"/>.
        /// </summary>
        public static DailyTimelineDataService Instance => instance ?? (instance = new DailyTimelineDataService());

        /// <summary>
        /// Gets or sets the value of pDaily Timeline page view model.
        /// </summary>
        public DailyTimelineViewModel DailyTimelineViewModel =>
            (this.dailyTimelineViewModel = PopulateData<DailyTimelineViewModel>("timeline.json"));
        #endregion

        #region Methods

        /// <summary>
        /// Populates the data for view model from json file.
        /// </summary>
        /// <typeparam name="T">Type of view model.</typeparam>
        /// <param name="fileName">Json file to fetch data.</param>
        /// <returns>Returns the view model object.</returns>
        private static T PopulateData<T>(string fileName)
        {
            string file = "TurnosFutbol.Data." + fileName;

            Assembly assembly = typeof(App).GetTypeInfo().Assembly;

            T obj;

            using (System.IO.Stream stream = assembly.GetManifestResourceStream(file))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                obj = (T)serializer.ReadObject(stream);
            }

            return obj;
        }
        #endregion
    }
}
