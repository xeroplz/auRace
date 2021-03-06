﻿using System;
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
using Newtonsoft.Json;

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

			// Get race.xml
			XmlDocument raceXML = new XmlDocument();
			var raceXMLPath = directory + "\\race.xml";

			// Get raceNA.xml
			XmlDocument raceNAXML = new XmlDocument();
			var raceNAXMLPath = directory + "\\raceNA.xml";

			// Get monster.xml
			XmlDocument monsterXML = new XmlDocument();
			var monsterXMLPath = directory + "\\monster.xml";

			// Get itemdroptype.json
			var itemDropTypePath = directory + "\\itemdroptype.json";

			try // Load XML files
			{
				raceXML.Load(raceXMLPath);
				raceNAXML.Load(raceNAXMLPath);
				monsterXML.Load(monsterXMLPath);
			}
			catch (Exception)
			{
				string errorMessage = string.Format("One or more of the required files could not be found.");
				MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				successLabel.Content = "Failed";
				return;
			}

			// check itemdroptype.json
			if (!File.Exists(itemDropTypePath))
			{
				string errorMessage = string.Format("One or more of the required files could not be found.");
				MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				successLabel.Content = "Failed";
				return;
			}

			List<string> lines = new List<string>();

			// Node Lists containing other XML values
			// Races NA
			var racesNA = raceNAXML.GetElementsByTagName("Race");

			// Monster
			var monsters = monsterXML.GetElementsByTagName("Monster");

			// Get Races
			var races = raceXML.GetElementsByTagName("Race");
			foreach (XmlElement race in races)
			{
				// Set Race Variables
				#region raceData
				var id = race.GetAttribute("ID");
				var name = race.GetAttribute("EnglishName");
				var group = race.GetAttribute("RaceDesc");
				var tags = race.GetAttribute("StringID") != "" ? race.GetAttribute("StringID") : "/none/";
				var gender = race.GetAttribute("Gender");
				var range = race.GetAttribute("MeleeAttackRange");
				var attackSpeed = race.GetAttribute("DefaultAttackSpeed");
				var knockCount = race.GetAttribute("DefaultDownHitCount");
				var splashRadius = race.GetAttribute("SplashRadius");
				var splashAngle = race.GetAttribute("SplashAngle");
				var splashDamage = race.GetAttribute("SplashDamage");
				var extraData = race.GetAttribute("ExtraData");
				var vehicleType = "0";
				var runSpeedFactor = "0";
				var state = "0";
				var stand = "0x03";
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
				var protection = "0.0";
				var sizeMin = "0.0";
				var sizeMax = "0.0";
				var combatPower = "0.0";
				var colorHex = "0";
				var c1 = "0x808080";
				var c2 = "0x808080";
				var c3 = "0x808080";
				var exp = "0";
				var goldMin = "0";
				var goldMax = "0";
				var elementPhysical = "0";
				var elementLightning = "0";
				var elementFire = "0";
				var elementIce = "0";
				var firstCreateStript = race.GetAttribute("FirstCreateStript");
				var eyeColor = "0";
				var eyeType = "0";
				var mouthType = "0";
				var skinColor = "0";
				var faceType = "0";
				var equip = "";
				var skills = "";
				#endregion

				// Skip Empty Value Line
				if (id == "")
					goto Skip;

				// Vehicle Type
				if (extraData != "")
				{
					if (extraData.Contains("vehicle type"))
					{
						vehicleType = Regex.Match(extraData, "vehicle type=\"(.*)\"").Groups[1].Value;
					}
				}

				// Get RunSpeedFactor from raceNA.xml
				foreach (XmlElement raceNAMatch in racesNA)
				{
					if (id == raceNAMatch.GetAttribute("ID"))
					{
						runSpeedFactor = raceNAMatch.GetAttribute("RunSpeedFactor");
					}
				}

				// State
				var stateTemp = race.GetAttribute("IsGoodNPC");
				if (stateTemp == "false")
				{
					state = "0x80000000";
				}
				else
				{
					state = "0xA0000000";
				}

				// Stand
				var isKnockBack = race.GetAttribute("ShovedEnable");
				var isKnockDown = race.GetAttribute("BlowAwayEnable");
				if ((isKnockBack == "false") && (isKnockDown == "false"))
					stand = "0x00";
				else if ((isKnockBack == "true") && (isKnockDown == "false"))
					stand = "0x01";
				else if ((isKnockBack == "false") && (isKnockDown == "true"))
					stand = "0x02";
				else if ((isKnockBack == "true") && (isKnockDown == "true"))
					stand = "0x03";

				// Get monster.xml values for variables
				foreach (XmlElement monsterMatch in monsters)
				{
					if (id == monsterMatch.GetAttribute("RaceID"))
					{
						attackMinBase = monsterMatch.GetAttribute("AttMin");
						attackMaxBase = monsterMatch.GetAttribute("AttMax");

						injuryMinBase = monsterMatch.GetAttribute("WAttMin");
						injuryMaxBase = monsterMatch.GetAttribute("WAttMax");

						level = monsterMatch.GetAttribute("Level");
						strength = monsterMatch.GetAttribute("Strength");
						intelligence = monsterMatch.GetAttribute("Intelligence");
						dexterity = monsterMatch.GetAttribute("Dexterity");
						will = monsterMatch.GetAttribute("Will");
						luck = monsterMatch.GetAttribute("Luck");
						life = monsterMatch.GetAttribute("Life");
						mana = monsterMatch.GetAttribute("Mana");
						stamina = monsterMatch.GetAttribute("Stamina");

						criticalBase = monsterMatch.GetAttribute("Critical") != "" ? monsterMatch.GetAttribute("Critical") : "0";
						balanceBase = monsterMatch.GetAttribute("Rate");
						defense = monsterMatch.GetAttribute("Defense");
						protection = string.Format("{0:F1}", double.Parse(monsterMatch.GetAttribute("Protect")));

						sizeMin = monsterMatch.GetAttribute("SizeMin") != "" ? string.Format("{0:F1}", double.Parse(monsterMatch.GetAttribute("SizeMin"))) : "0";
						sizeMax = monsterMatch.GetAttribute("SizeMax") != "" ? string.Format("{0:F1}", double.Parse(monsterMatch.GetAttribute("SizeMax"))) : "0";

						combatPower = string.Format("{0:F1}", double.Parse(monsterMatch.GetAttribute("CombatPower2")));
						colorHex = monsterMatch.GetAttribute("Color_Hex");

						exp = monsterMatch.GetAttribute("ParticipationExp") != "" ? monsterMatch.GetAttribute("ParticipationExp") : "0";
						goldMin = monsterMatch.GetAttribute("BonusMoneyMin") != "" ? monsterMatch.GetAttribute("BonusMoneyMin") : "0";
						goldMax = monsterMatch.GetAttribute("BonusMoneyMax") != "" ? monsterMatch.GetAttribute("BonusMoneyMax") : "0";

						elementPhysical = monsterMatch.GetAttribute("ElementPhysical") != "" ? monsterMatch.GetAttribute("ElementPhysical") : "0";
						elementLightning = monsterMatch.GetAttribute("ElementLightning") != "" ? monsterMatch.GetAttribute("ElementLightning") : "0";
						elementFire = monsterMatch.GetAttribute("ElementFire") != "" ? monsterMatch.GetAttribute("ElementFire") : "0";
						elementIce = monsterMatch.GetAttribute("ElementIce") != "" ? monsterMatch.GetAttribute("ElementIce") : "0";
					}
				}

				// Variable Color not Implemented
				/*
				if (colorHex.Contains("|")) // Color may vary
				{

				}
				*/

				// Color Hex
				if (colorHex.Contains("c1"))
				{
					var match1 = Regex.Match(colorHex, "c1:([a-f0-9]+)", RegexOptions.IgnoreCase);
					if (match1.Success)
						c1 = "0x" + match1.Groups[1].Value;
				}

				if (colorHex.Contains("c2")) // Between c2:[x] c3
				{
					var match2 = Regex.Match(colorHex, "c2:([a-f0-9]+)", RegexOptions.IgnoreCase);
					if (match2.Success)
						c2 = "0x" + match2.Groups[1].Value;
				}

				if (colorHex.Contains("c3")) // Between c3:[x]"
				{
					var match3 = Regex.Match(colorHex, "c3:([a-f0-9]+)", RegexOptions.IgnoreCase);
					if (match3.Success)
						c3 = "0x" + match3.Groups[1].Value;
				}

				// Set Face
				var setFaceMatch = Regex.Match(firstCreateStript, @"setface(.*?\))");
				if (setFaceMatch.Success)
				{
					var setFace = setFaceMatch.Groups[1].Value;

					// Eye Color Set
					if (setFace.Contains("ec:"))
					{
						var eyeColorMatch = Regex.Match(setFace, "ec:([a-f0-9]+)", RegexOptions.IgnoreCase);
						if (eyeColorMatch.Success)
							eyeColor = eyeColorMatch.Groups[1].Value;
					}

					// Eye Type Set
					if (setFace.Contains("et:"))
					{
						var eyeTypeMatch = Regex.Match(setFace, @"et:(.*? |.*?\))", RegexOptions.IgnoreCase);

						if (eyeTypeMatch.Success)
						{
							eyeType = eyeTypeMatch.Groups[1].Value;
							eyeType = eyeType.Trim("et: )".ToCharArray());

							if (eyeType.Contains('|'))
								eyeType = eyeType.Replace('|', ',');
						}
					}

					// Mouth Type Set
					if (setFace.Contains("mt:"))
					{
						var mouthTypeMatch = Regex.Match(setFace, @"mt:(.*? |.*?\))", RegexOptions.IgnoreCase);

						if (mouthTypeMatch.Success)
						{
							mouthType = mouthTypeMatch.Groups[1].Value;
							mouthType = mouthType.Trim("mt: )".ToCharArray());

							if (mouthType.Contains('|'))
								mouthType = mouthType.Replace('|', ',');
						}
					}

					// Skin Color Set
					if (setFace.Contains("sc:"))
					{
						var skinColorMatch = Regex.Match(setFace, @"sc:(.*? |.*?\))", RegexOptions.IgnoreCase);

						if (skinColorMatch.Success)
						{
							skinColor = skinColorMatch.Groups[1].Value;
							skinColor = skinColor.Trim("sc: )".ToCharArray());

							if (skinColor.Contains('|'))
								skinColor = skinColor.Replace('|', ',');
						}
					}

					// Face Type Set - To be used in Equips
					if (setFace.Contains("ft:"))
					{
						var faceTypeMatch = Regex.Match(setFace, @"ft:(.*? |.*?\))", RegexOptions.IgnoreCase);

						if (faceTypeMatch.Success)
						{
							faceType = faceTypeMatch.Groups[1].Value;
							faceType = faceType.Trim("mt: )".ToCharArray());

							if (faceType.Contains('|'))
								faceType = faceType.Replace('|', ',');
						}
					}
				}

				// Equipment
				var equipMatches = Regex.Matches(firstCreateStript, @"additemex(.*?;)");
				foreach (Match equipMatch in equipMatches)
				{
					if (equipMatch.Success)
					{
						// Get Raw Equip Data
						var equipString = equipMatch.Groups[1].Value;
						equipString = equipString.Replace("additemex", "");

						var itemString = "";
						var color1String = "0x808080";
						var color2String = "0x808080";
						var color3String = "0x808080";
						var pocketString = "0";

						// Item Id(s)
						var itemMatch = Regex.Match(equipString, @"id:(.*? |.*?\))");
						if (itemMatch.Success)
						{
							itemString = itemMatch.Groups[1].Value;
							itemString = itemString.Replace("id:", "");

							if (itemString.Contains('|'))
							{
								itemString = itemString.Replace('|', ',');
								itemString = "[" + itemString + "]";
							}
						}

						// Color1(s)
						var color1Match = Regex.Match(equipString, @"col1:(.*? |.*?\))");
						if (color1Match.Success)
						{
							color1String = color1Match.Groups[1].Value;
							color1String = color1String.Replace("col1:", "");
							color1String = "0x" + color1String;

							if (color1String.Contains('|'))
							{
								color1String = color1String.Replace("|", ",0x");
								color1String = "[" + color1String + "]";
							}
						}

						// Color2(s)
						var color2Match = Regex.Match(equipString, @"col2:(.*? |.*?\))");
						if (color2Match.Success)
						{
							color2String = color2Match.Groups[1].Value;
							color2String = color2String.Replace("col2:", "");
							color2String = "0x" + color2String;

							if (color2String.Contains('|'))
							{
								color2String = color2String.Replace("|", ",0x");
								color2String = "[" + color2String + "]";
							}
						}

						// Color3(s)
						var color3Match = Regex.Match(equipString, @"col3:(.*? |.*?\))");
						if (color3Match.Success)
						{
							color3String = color3Match.Groups[1].Value;
							color3String = color3String.Replace("col3:", "");
							color3String = "0x" + color3String;

							if (color3String.Contains('|'))
							{
								color3String = color3String.Replace("|", ",0x");
								color3String = "[" + color3String + "]";
							}
						}

						// Pocket
						var pocketMatch = Regex.Match(equipString, "pocket:([a-f0-9]+)");
						if (pocketMatch.Success)
						{
							pocketString = pocketMatch.Groups[1].Value;
							pocketString = pocketString.Replace("pocket:", "");
						}

						// Remove Extra Possible Characters When Finished
						itemString = itemString.Trim(' ', '(', ')');
						color1String = color1String.Trim(' ', '(', ')');
						color2String = color2String.Trim(' ', '(', ')');
						color3String = color3String.Trim(' ', '(', ')');
						pocketString = pocketString.Trim(' ', '(', ')');

						equip += "{itemId: " + itemString + ", pocket: " + pocketString + ", color1: " + color1String + ", color2: " + color2String + ", color3: " + color3String + "}, ";
					}
				}

				// Apply Face With SkinColor
				if (faceType != "0")
				{
					equip += "{itemId: " + faceType + ", pocket: 3, color1: " + skinColor + "}, ";
				}

				// Skills
				var skillMatches = Regex.Matches(firstCreateStript, @"setmonsterskill(.*?;)");
				foreach (Match skillMatch in skillMatches)
				{
					// Get Raw Skill Data
					var skillString = skillMatch.Groups[1].Value;
					skillString = skillString.Replace("setmonsterskill", "");

					var skillId = "";
					var skillRank = "";

					// Skill Id
					var skillIdMatch = Regex.Match(skillString, @"(.*?([a-f0-9]+) |([a-f0-9]+)),");
					if (skillIdMatch.Success)
					{
						skillId = skillIdMatch.Groups[1].Value;
						skillId = skillId.Trim(',', ' ', '(');
					}

					// Skill Rank
					var skillRankMatch = Regex.Match(skillString, @",(.*? ([a-f0-9]+)|([a-f0-9]+))");
					if (skillRankMatch.Success)
					{
						skillRank = skillRankMatch.Groups[1].Value;
						skillRank = skillRank.Trim(',', ' ', ')');

						// Rank Values
						switch (skillRank)
						{
							case "1":
								skillRank = "F";
								break;
							case "2":
								skillRank = "E";
								break;
							case "3":
								skillRank = "D";
								break;
							case "4":
								skillRank = "C";
								break;
							case "5":
								skillRank = "B";
								break;
							case "6":
								skillRank = "A";
								break;
							case "7":
								skillRank = "9";
								break;
							case "8":
								skillRank = "8";
								break;
							case "9":
								skillRank = "7";
								break;
							case "10":
								skillRank = "6";
								break;
							case "11":
								skillRank = "5";
								break;
							case "12":
								skillRank = "4";
								break;
							case "13":
								skillRank = "3";
								break;
							case "14":
								skillRank = "2";
								break;
							case "15":
								skillRank = "1";
								break;
							default:
								skillRank = "N";
								break;
						}
					}

					skills += "{skillId: " + skillId + ", rank: " + inQuotes(skillRank) + "}, ";
				}

				// Drops
				var desRaceDrops = JsonConvert.DeserializeObject(File.ReadAllText(itemDropTypePath));
				var raceDrops = JsonConvert.SerializeObject(desRaceDrops);

				// Create String
				var raceText = ("{" +
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
					"color1: " + c1 + ", " +
					"color2: " + c2 + ", " +
					"color3: " + c3 + ", " +
					"sizeMin: " + sizeMin + ", " +
					"sizeMax: " + sizeMax + ", " +
					"level: " + level + ", " +
					"str: " + strength + ", " +
					"int: " + intelligence + ", " +
					"dex: " + dexterity + ", " +
					"will: " + will + ", " +
					"luck: " + luck + ", " +
					"cp: " + combatPower + ", " +
					"life: " + life + ", " +
					"mana: " + mana + ", " +
					"stamina: " + stamina + ", " +
					"defense: " + defense + ", " +
					"protection: " + protection + ", " +
					"elementPhysical: " + elementPhysical + ", " +
					"elementLightning: " + elementLightning + ", " +
					"elementFire: " + elementFire + ", " +
					"elementIce: " + elementIce + ", " +
					"exp: " + exp + ", " +
					"goldMin: " + goldMin + ", " +
					"goldMax: " + goldMax + ""
					);

				// Optional Values
				// Eye Color
				var sizeMaxString = "sizeMax: " + sizeMax + ", ";
				var sizeMaxIndex = raceText.IndexOf(sizeMaxString);
				if (eyeColor != "0")
					raceText = raceText.Insert(sizeMaxIndex + sizeMaxString.Length, "eyeColor: " + eyeColor + ", ");

				// Eye Type
				if (eyeType != "0")
				{
					if (eyeType.Contains(",")) // Create selectable version
						raceText = raceText.Insert(sizeMaxIndex + sizeMaxString.Length, "eyeType: [" + eyeType + "], ");
					else // Singular version
						raceText = raceText.Insert(sizeMaxIndex + sizeMaxString.Length, "eyeType: " + eyeType + ", ");
				}

				// Mouth Type
				if (mouthType != "0")
				{
					if (mouthType.Contains(",")) // Create selectable version
						raceText = raceText.Insert(sizeMaxIndex + sizeMaxString.Length, "mouthType: [" + mouthType + "], ");
					else // Singular version
						raceText = raceText.Insert(sizeMaxIndex + sizeMaxString.Length, "mouthType: " + mouthType + ", ");
				}

				// Skin Color
				if (skinColor != "0")
				{
					if (skinColor.Contains(",")) // Create selectable version
						raceText = raceText.Insert(sizeMaxIndex + sizeMaxString.Length, "skinColor: [" + skinColor + "], ");
					else // Singular version
						raceText = raceText.Insert(sizeMaxIndex + sizeMaxString.Length, "skinColor: " + skinColor + ", ");
				}

				// Skills
				if (skills != "")
				{
					raceText += ", skills: [" + skills + "] ";
				}

				// Equipment
				if (equip != "")
				{
					raceText += ", equip: [" + equip + "] ";
				}

				// Close Entry
				raceText += "},";

				lines.Add(raceText);

				Skip:
				lines = lines;
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
