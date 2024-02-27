using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StoichiometryLibrary
{
    /// <summary>
    /// Static class to get periodic table elements
    /// </summary>
    public static class PeriodicTable
    {
        private static int NumberOfElements = 119;

        private static IElement[] _elements = new IElement[NumberOfElements];

        /// <summary>
        /// Elements in periodic table property
        /// </summary>
        public static IElement[] Elements => elementsPopulated ? _elements : GetElementsFromJSON();

        /// <summary>
        /// flag that signals if Elements propety is already populated
        /// </summary>
        private static bool elementsPopulated = false;

        /// <summary>
        /// Read a Json file of peridoc table elements and parse it into .NET array
        /// </summary>
        /// <returns>Array of IElement</returns>
        /// <exception cref="FileNotFoundException">File not found</exception>
        /// <exception cref="IOException">Reading file</exception>
        /// <exception cref="JsonSerializationException">Converting JSON</exception>
        /// <exception cref="JsonReaderException">Reading JSON</exception>
        /// <exception cref="Exception">other exception</exception>
        private static IElement[] GetElementsFromJSON()
        {
            try
            {
                // File path
                string periodicTableJsonFile = "../../../../PeriodicTableJSON.json";

                // Contents of File
                string periodicTableText = File.ReadAllText(periodicTableJsonFile);

                // Parse the file to json object
                JObject elementsObject = JObject.Parse(periodicTableText);

                // get the json array ("elemets" in file)
                JArray elementsArray = (JArray)elementsObject["elements"]!;

                // convert json to .NET array of Element
                IElement[] elements = elementsArray.ToObject<IList<Element>>()!.ToArray();

                // flag that list is popluated
                elementsPopulated = true;

                _elements = elements;
                // return the array of elements
                return elements;
            }
            // exceptions
            catch(FileNotFoundException) {throw new FileNotFoundException("Can't find Periodic Table file");}
            catch (IOException) { throw new IOException("Can't read file"); }
            catch(JsonReaderException) { throw new JsonReaderException("Can't read JSON"); }
            catch (JsonSerializationException) { throw new JsonSerializationException("Can't convert JSON"); }
            catch (Exception) { throw new("Error"); }
        }
    }
}
