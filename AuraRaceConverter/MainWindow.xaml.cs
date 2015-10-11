using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;

namespace AuraRaceConverter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void creditsButton_Click(object sender, RoutedEventArgs e)
		{
			Credits creditsWindow = new Credits();
			creditsWindow.Show();
		}

		private void start_Button_Click(object sender, RoutedEventArgs e)
		{
			var directory = Directory.GetCurrentDirectory().ToString();

			// Check race.xml
			XmlDocument raceXML = new XmlDocument();
			var raceXMLPath = directory + "\\race.xml";

			try
			{
				raceXML.Load(raceXMLPath);
			}
			catch (Exception)
			{
				string errorMessage = string.Format("race.xml could not be found.");
				MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Check raceNA.xml
			XmlDocument raceNAXML = new XmlDocument();
			var raceNAXMLPath = directory + "\\raceNA.xml";

			try
			{
				raceNAXML.Load(raceNAXMLPath);
			}
			catch (Exception)
			{
				string errorMessage = string.Format("raceNA.xml could not be found.");
				MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Check monster.xml
			XmlDocument monsterXML = new XmlDocument();
			var monsterXMLPath = directory + "\\monster.xml";

			try
			{
				monsterXML.Load(monsterXMLPath);
			}
			catch (Exception)
			{
				string errorMessage = string.Format("monster.xml could not be found.");
				MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			List<string> lines = new List<string>();

			successLabel.Content = "Working...";

			// Get Races
			var races = raceXML.GetElementsByTagName("Race");
			foreach (XmlElement race in races)
			{
				var id = race.GetAttribute("ID");
				var name = race.GetAttribute("EnglishName");
				var group = race.GetAttribute("RaceDesc");
				var tags = race.GetAttribute("StringID");
				var gender = race.GetAttribute("Gender");

				// Vehicle Type
				var extraData = race.GetAttribute("ExtraData");
				var vehicleType = "0";
				/*
				if (extraData != "")
				{
					XmlDocument extraDataXml = new XmlDocument();
					extraData.Replace("&quot;", "\"");
					extraData.Replace("&lt;", "<");
					extraData.Replace("&gt;", ">");
					extraDataXml.Load(extraData);
					var extraElements = extraDataXml.GetElementsByTagName("xml");
					foreach (XmlElement extraElement in extraElements)
					{
						vehicleType = (extraElement.GetAttribute("vehicle type"));
					}
				}
				*/

				// Get RunSpeedFactor from raceNA.xml
				var runSpeedFactor = "0";
				var racesNA = raceNAXML.GetElementsByTagName("Race");
				foreach (XmlElement raceNA in racesNA)
				{
					if (id == raceNA.GetAttribute("ID")) // If same race in both files
					{
						runSpeedFactor = raceNA.GetAttribute("RunSpeedFactor");
					}
				}

				// State
				var stateTemp = race.GetAttribute("IsGoodNPC");
				string state = "";
				if (stateTemp == "false")
				{
					state = "0x80000000";
				}
				else
				{
					state = "0xA0000000";
				}

				var invWidth = race.GetAttribute("InventorySizeX");
				var invHeight = race.GetAttribute("InventorySizeY");

				var raceText=("{ " + 
					"id: " + id + ", " +
					"name: " + inQuotes(name) + ", " +
                    "group: " + inQuotes(group) + ", " +
                    "tags: " + inQuotes(tags) + ", " +
					"gender: " + gender + ", " +
					"vehicleType: " + vehicleType + ", " + 
					"runSpeedFactor: " + runSpeedFactor + ", " + 
					"state: " + state + ", " + "},");

				lines.Add(raceText);
			}

			File.WriteAllLines((directory + "\\races.txt"), lines);
			successLabel.Content = "Success!";
		}

		private string inQuotes (string text)
		{
			return ("\"") + text + ("\"");
		}
	}
}
