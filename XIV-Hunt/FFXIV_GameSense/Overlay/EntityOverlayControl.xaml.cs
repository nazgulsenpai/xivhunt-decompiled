using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace FFXIV_GameSense.Overlay
{
	public partial class EntityOverlayControl : UserControl
	{
		private EntityOverlayControlViewModel Model { get; set; }

		public EntityOverlayControl()
		{
			this.InitializeComponent();
		}

		public EntityOverlayControl(Entity c, bool IsSelf = false)
		{
			this.InitializeComponent();
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			this.Model = new EntityOverlayControlViewModel
			{
				NameColor = this.GetColor(c, IsSelf),
				Name = c.Name,
				Icon = EntityOverlayControl.GetIcon(c)
			};
			base.DataContext = this.Model;
		}

		private Brush GetColor(Entity c, bool IsSelf)
		{
			if (c is PC)
			{
				return new SolidColorBrush(IsSelf ? Colors.LightGreen : Colors.LightBlue);
			}
			if (c is Monster)
			{
				HuntRank hr;
				if (Hunt.TryGetHuntRank(((Monster)c).BNpcNameID, out hr))
				{
					return new SolidColorBrush((hr == HuntRank.B) ? Color.FromArgb(byte.MaxValue, 0, 0, 231) : Colors.Red);
				}
				return new SolidColorBrush(Colors.White);
			}
			else
			{
				if (string.IsNullOrWhiteSpace(c.Name))
				{
					return new SolidColorBrush(Colors.MediumPurple);
				}
				return new SolidColorBrush(Colors.LightGray);
			}
		}

		public string GetEntityName()
		{
			return this.Model.Name;
		}

		private void SetNameColor(Brush brush)
		{
			this.Model.NameColor = brush;
		}

		private void RotateImage(float angle)
		{
			RotateTransform rotateTransform = new RotateTransform((double)angle);
			this.image.RenderTransform = rotateTransform;
		}

		public void Update(Entity c)
		{
			this.Model.Name = ((!string.IsNullOrWhiteSpace(c.Name)) ? c.Name : (c.GetType().Name + " No Name"));
			if (c is PC)
			{
				this.RotateImage(-c.HeadingDegree);
				return;
			}
			if (c is EObject)
			{
				this.Model.Icon = EntityOverlayControl.GetIcon(c);
				if (string.IsNullOrWhiteSpace(c.Name) && ((EObject)c).SubType == EObjType.Hoard)
				{
					this.Model.Name = "Hoard!";
					return;
				}
			}
			else if (c is Combatant && ((Combatant)c).CurrentHP == 0u)
			{
				base.Visibility = Visibility.Hidden;
			}
		}

		private static string GetIcon(Entity c)
		{
			if (c is PC)
			{
				return EntityOverlayControl.IconUris[typeof(PC).Name];
			}
			if (c is Monster)
			{
				return EntityOverlayControl.IconUris[typeof(Monster).Name];
			}
			if (c is Aetheryte)
			{
				return EntityOverlayControl.IconUris[typeof(Aetheryte).Name];
			}
			if (c is Treasure || (c is EObject && ((EObject)c).SubType == EObjType.BronzeTrap))
			{
				return EntityOverlayControl.IconUris[typeof(Treasure).Name];
			}
			if (c is EObject)
			{
				if (((EObject)c).SubType == EObjType.CairnOfPassage || ((EObject)c).SubType == EObjType.BeaconOfPassage)
				{
					return EntityOverlayControl.IconUris[EObjType.CairnOfPassage.ToString() + (((EObject)c).CairnIsUnlocked ? "Unlocked" : string.Empty)];
				}
				if (((EObject)c).SubType == EObjType.CairnOfReturn || ((EObject)c).SubType == EObjType.BeaconOfReturn)
				{
					return EntityOverlayControl.IconUris[EObjType.CairnOfReturn.ToString() + (((EObject)c).CairnIsUnlocked ? "Unlocked" : string.Empty)];
				}
				if (((EObject)c).SubType == EObjType.Silver)
				{
					return EntityOverlayControl.IconUris[EObjType.Silver.ToString()];
				}
				if (((EObject)c).SubType == EObjType.Gold)
				{
					return EntityOverlayControl.IconUris[EObjType.Gold.ToString()];
				}
				if (((EObject)c).SubType == EObjType.Banded || ((EObject)c).SubType == EObjType.Hoard)
				{
					return EntityOverlayControl.IconUris[EObjType.Banded.ToString()];
				}
			}
			if (c is NPC)
			{
				return EntityOverlayControl.IconUris[typeof(NPC).Name];
			}
			return "/Resources/Images/ui/uld/image2.tex.png";
		}

		private const string icondir = "/Resources/Images/ui/icon/060000/";

		private static readonly Dictionary<string, string> IconUris = new Dictionary<string, string>
		{
			{
				typeof(PC).Name,
				"/Resources/Images/ui/icon/060000/060443.tex.png"
			},
			{
				typeof(NPC).Name,
				"/Resources/Images/NPC.png"
			},
			{
				typeof(Monster).Name,
				"/Resources/enemy.ico"
			},
			{
				typeof(Treasure).Name,
				"/Resources/Images/ui/icon/060000/060356.tex.png"
			},
			{
				"Silver",
				"/Resources/Images/ui/icon/060000/060355.tex.png"
			},
			{
				"Gold",
				"/Resources/Images/ui/icon/060000/060354.tex.png"
			},
			{
				typeof(Aetheryte).Name,
				"/Resources/Images/ui/icon/060000/060453.tex.png"
			},
			{
				"CairnOfReturn",
				"/Resources/Images/ui/icon/060000/060905.tex.png"
			},
			{
				"CairnOfReturnUnlocked",
				"/Resources/Images/ui/icon/060000/060906.tex.png"
			},
			{
				"CairnOfPassage",
				"/Resources/Images/ui/icon/060000/060907.tex.png"
			},
			{
				"CairnOfPassageUnlocked",
				"/Resources/Images/ui/icon/060000/060908.tex.png"
			},
			{
				"Banded",
				"/Resources/Images/Banded.png"
			}
		};
	}
}
