using Alpaca.Markets;
using StratFlux.ModelEnums;
using System.Data;
using System.Runtime.InteropServices;

namespace StratFlux.Services
{
    // This class simply exists so that the backtesting engine (when instantiated) can have access to the needed keys
    // for the alpaca markets API without exposing the keys themselves in the source code
    public class AlpacaDataService
    {
        private IAlpacaDataClient _dataClient;

        public AlpacaDataService(string alpacaApiKey, string alpacaSecretKey)
        {
            // The two keys given in the constructor allow the client to be created
            _dataClient = Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(new SecretKey(alpacaApiKey, alpacaSecretKey));
        }

        public async Task<IBar[]?> GetData(string stockToTrade, DateTime timeFrom, DateTime timeTo, TimeResolution timeResolution)
        {
            Dictionary<TimeResolution, BarTimeFrame> timeFrameMappings = new Dictionary<TimeResolution, BarTimeFrame>()
            {
                { TimeResolution.PerMinute, BarTimeFrame.Minute },
                { TimeResolution.Hourly, BarTimeFrame.Hour },
                { TimeResolution.Daily, BarTimeFrame.Day },
                { TimeResolution.Weekly, BarTimeFrame.Week },
                { TimeResolution.Monthly, BarTimeFrame.Month }
            };

            Dictionary<TimeResolution, int> timeResolutionMultiplierMappings = new Dictionary<TimeResolution, int>()
            {
                { TimeResolution.PerMinute, 1 },
                { TimeResolution.Hourly, 60 },
                { TimeResolution.Daily, 1440 },
                { TimeResolution.Weekly, 10080 },
                { TimeResolution.Monthly, 312480 }
            };

            DateTime tempTimeTo = timeTo;
            DateTime tempTimeFrom = timeFrom.AddMinutes(-(100 * timeResolutionMultiplierMappings[timeResolution]));

            
            HistoricalBarsRequest dataRequest = new HistoricalBarsRequest(stockToTrade.ToUpper(), tempTimeFrom, tempTimeTo, timeFrameMappings[timeResolution]);

            // The rawData is an IEnumerable and is not easy to read from, therefore, it is converted into an array
            var rawData = await _dataClient.ListHistoricalBarsAsync(dataRequest);
            IBar[] data = rawData.Items.ToArray();

            // Actual start position is the position in the array where the specified time frame start is first passed
            int actualStartPosition = EvaluateActualStartPosition(data, timeFrom);

            // startPositionComparison is used to check if the actual start position hasn't changed while in the loop
            int startPositionComparison = 0;
            int numberOfLoops = 0;

            // This loop will run until there are 50 or more values which precede the wanted starting value
            while (actualStartPosition < 50)
            {
                // For each loop, the number of loops increases
                numberOfLoops++;

                // If there have been over 100 loops, it is likely that the required data will not be found
                // Therefore, null will be returned indicating the time frame is invalid
                if (numberOfLoops > 100) { return null; }

                // This sets the new timeTo value to be the old timeFrom
                tempTimeTo = tempTimeFrom;

                // The old timeFrom will be pushed back by the time resolution in minutes multiplied by 100
                tempTimeFrom = tempTimeFrom.AddMinutes(-(100 * timeResolutionMultiplierMappings[timeResolution]));
                
                // This uses the updated time values to get the market data
                dataRequest = new HistoricalBarsRequest(stockToTrade.ToUpper(), tempTimeFrom, tempTimeTo, timeFrameMappings[timeResolution]);
                rawData = await _dataClient.ListHistoricalBarsAsync(dataRequest);

                // The old array of data and the new array is concatenated so that together, they are one big array
                data = rawData.Items.ToArray().Concat(data).ToArray();



                /*
                If the actual start position hasn't changed, this indicates the timeFrom in the
                data request hasn't been pushed back far enough.

                Therefore, when the actual start position hasn't changed, the same operations that
                happen in a normal loop will run except that the multiplier for pushing back the
                timeFrom value in the data request will be 500 instead of 100.
                */
                startPositionComparison = actualStartPosition;
                actualStartPosition = EvaluateActualStartPosition(data, timeFrom);

                if (startPositionComparison == actualStartPosition)
                {
                    // Number of loops is increased and checked because this case counts as a new loop
                    numberOfLoops++;
                    if (numberOfLoops > 100) { return null; }

                    // Same operations as a normal loop take place but multiplier is 500 instead of 100
                    tempTimeTo = tempTimeFrom;
                    tempTimeFrom = tempTimeFrom.AddMinutes(-(500 * timeResolutionMultiplierMappings[timeResolution]));

                    dataRequest = new HistoricalBarsRequest(stockToTrade.ToUpper(), tempTimeFrom, tempTimeTo, timeFrameMappings[timeResolution]);

                    rawData = await _dataClient.ListHistoricalBarsAsync(dataRequest);

                    data = rawData.Items.ToArray().Concat(data).ToArray();

                    actualStartPosition = EvaluateActualStartPosition(data, timeFrom);
                }
            }

            // This take the subarray of the data which includes the 50 data values preceding the actual start
            int startPosition = actualStartPosition - 50;
            data = SubArray(data, startPosition, data.Length - 1);

            // All required data has been retrieved so return it
            return data;
        }

        // Actual start position is the index in the array where the time frame start is first passed
        // This function finds the actual start position based on the DateTime and the array
        private int EvaluateActualStartPosition(IBar[] data, DateTime actualStartTime)
        {
            // Each bar in the array is looped over and the index of the bar which first exists
            // after the specified date time will be returned
            for (int i = 0; i < data.Length; i++)
            {
                IBar bar = data[i];
                int timeComparison = DateTime.Compare(actualStartTime, bar.TimeUtc);

                if (timeComparison <= 0)
                {
                    return i;
                }
            }

            /*
            This is just a base case so a value will be returned but this point will only be reached in the event of an error.
            If there is an error, this will likely be repeatedly called and returned causing the while loop base case to stop
            the process and notify the user.
            */
            return 0;
        }

        // This function returns a sub array of IBar[] from the specified position to the end of the original array
        // It is public and static so the method can be used in the strategy program without the need for an instance
        public static IBar[] SubArray(IBar[] array, int positionFrom, int positionTo)
        {
            IBar[] subArray = new IBar[positionTo - positionFrom + 1];

            for (int i = positionFrom; i < positionTo + 1; i++)
            {
                subArray[i - positionFrom] = array[i];
            }

            return subArray;
        }
    }
}