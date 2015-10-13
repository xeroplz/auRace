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
using System.Text.RegularExpressions;

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

			successLabel.Content = "Working...";
			List<string> lines = new List<string>();

			// Get Races
			var races = raceXML.GetElementsByTagName("Race");
			foreach (XmlElement race in races)
			{
				var id = race.GetAttribute("ID");
				var name = race.GetAttribute("EnglishName");
				var group = race.GetAttribute("RaceDesc");
				var tags = race.GetAttribute("StringID");
				var gender = race.GetAttribute("Gender");
				var range = race.GetAttribute("MeleeAttackRange");
				var attackSpeed = race.GetAttribute("DefaultAttackSpeed");
				var knockCount = race.GetAttribute("DefaultDownHitCount");
				var splashRadius = race.GetAttribute("SplashRadius");
				var splashAngle = race.GetAttribute("SplashAngle");
				var splashDamage = race.GetAttribute("SplashDamage");

				// Vehicle Type
				var extraData = race.GetAttribute("ExtraData");
				var vehicleType = "0";
				if (extraData != "")
				{
					if (extraData.Contains("vehicle type"))
					{
						vehicleType = Regex.Match(extraData, "vehicle type=\"(.*)\"").Groups[1].Value;
					}
				}

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
				var state = "0";
				if (stateTemp == "false")
				{
					state = "0x80000000";
				}
				else
				{
					state = "0xA0000000";
				}

				// Stand
				var stand = "0x03";
				var isKnockBack = race.GetAttribute("ShovedEnable");
				var isKnockDown = race.GetAttribute("BlowAwayEnable");
				if ((isKnockBack == "true") && (isKnockDown == "false"))
				{
					stand = "0x01";
				}
				else if ((isKnockBack == "false") && (isKnockDown == "true"))
				{
					stand = "0x02";
				}
				else if ((isKnockBack == "true") && (isKnockDown == "true"))
				{
					stand = "0x03";
				}

				var invWidth = race.GetAttribute("InventorySizeX");
				var invHeight = race.GetAttribute("InventorySizeY");

				var attackMinBase = "0";
				var attackMaxBase = "0";
				var attackMinBaseMod = "0";
				var attackMaxBaseMod = "0";
				var injuryMinBase = "0";
				var injuryMaxBase = "0";
				var InjuryMinBaseMod = "0";
				var InjuryMaxBaseMod = "0";
				var level = "0";
				var strength = "0";
				var intelligence = "0";
				var dexterity = "0";
				var will = "0";
				var luck = "0";
				var life = "0";
				var mana = "0";
				var stamina = "0";
				var criticalBase = "0";
				var criticalBaseMod = "0";
				var balanceBase = "0";
				var balanceBaseMod = "0";
				var defense = "0";
				var protection = "0";
				var sizeMin = "0";
				var sizeMax = "0";
				var combatPower = "0";
				var colorHex = "0";
				var exp = "0";
				var goldMin = "0";
				var goldMax = "0";
				var elementPhysical = "0";
				var elementLightning = "0";
				var elementFire = "0";
				var elementIce = "0";

				// Get monster.xml values for above variables
				var monsters = monsterXML.GetElementsByTagName("Monster");
				foreach (XmlElement monster in monsters)
				{
					if (id == monster.GetAttribute("RaceID")) // If same race
					{
						attackMinBase = monster.GetAttribute("AttMin");
						attackMaxBase = monster.GetAttribute("AttMax");

						injuryMinBase = monster.GetAttribute("WAttMin");
						injuryMaxBase = monster.GetAttribute("WAttMax");

						level = monster.GetAttribute("Level");
						strength = monster.GetAttribute("Strength");
						intelligence = monster.GetAttribute("Intelligence");
						dexterity = monster.GetAttribute("Dexterity");
						will = monster.GetAttribute("Will");
						luck = monster.GetAttribute("Luck");
						life = monster.GetAttribute("Life");
						mana = monster.GetAttribute("Mana");
						stamina = monster.GetAttribute("Stamina");

						criticalBase = monster.GetAttribute("Critical");
						balanceBase = monster.GetAttribute("Rate");
						defense = monster.GetAttribute("Defense");
						protection = monster.GetAttribute("Protect");

						sizeMin = monster.GetAttribute("SizeMin");
						sizeMax = monster.GetAttribute("SizeMax");

						combatPower = monster.GetAttribute("CombatPower2");
						colorHex = monster.GetAttribute("Color_Hex");

						exp = monster.GetAttribute("ParticipationExp");
						goldMin = monster.GetAttribute("BonusMoneyMin");
						goldMax = monster.GetAttribute("BonusMoneyMax");

						elementPhysical = monster.GetAttribute("ElementPhysical");
						elementLightning = monster.GetAttribute("ElementLightning");
						elementFire = monster.GetAttribute("ElementFire");
						elementIce = monster.GetAttribute("ElementIce");
					}
				}

				// Create String
				var raceText = ("{ " +
					"id: " + id + ", " +
					"name: " + inQuotes(name) + ", " +
					"group: " + inQuotes(group) + ", " +
					"tags: " + inQuotes(tags) + ", " +
					"gender: " + gender + ", " +
					"vehicleType: " + vehicleType + ", " +
					"runSpeedFactor: " + runSpeedFactor + ", " +
					"state: " + state + ", " +
					"invWidth: " + invWidth + ", " +
					"invHeight: " + invHeight + ", " +
					"attackMinBase: " + attackMinBase + ", " +
					"attackMaxBase: " + attackMaxBase + ", " +
					"attackMinBaseMod: " + attackMinBaseMod + ", " +
					"attackMaxBaseMod: " + attackMaxBaseMod + ", " +
					"injuryMinBase: " + injuryMinBase + ", " +
					"injuryMaxBase: " + injuryMaxBase + ", " +
					"injuryMinBaseMod: " + InjuryMinBaseMod + ", " +
					"injuryMaxBaseMod: " + InjuryMaxBaseMod + ", " +
					"range: " + range + ", " +
					"attackSpeed: " + attackSpeed + ", " +
					"knockCount: " + knockCount + ", " +
					"criticalBase: " + criticalBase + ", " +
					"criticalBaseMod: " + criticalBaseMod + ", " +
					"balanceBase: " + balanceBase + ", " +
					"balanceBaseMod: " + balanceBaseMod + ", " +
					"splashRadius: " + splashRadius + ", " +
					"splashAngle: " + splashAngle + ", " +
					"splashDamage: " + splashDamage + ", " +
					"stand: " + stand + ", " +
					"ai: \"none\"" + ", " + // Ai none until specified
					"},");

				lines.Add(raceText);
			}

			File.WriteAllLines((directory + "\\races.txt"), lines);
			successLabel.Content = "Success!";
		}

		private string inQuotes(string text)
		{
			return ("\"") + text + ("\"");
		}
	}
}
