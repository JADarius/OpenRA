using System;
using System.Collections.Generic;
using System.Text;
using OpenRa.TechTree;
using BluntDirectX.Direct3D;
using OpenRa.FileFormats;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OpenRa.Game
{
	class Sidebar
	{
		TechTree.TechTree techTree = new TechTree.TechTree();

		SpriteRenderer spriteRenderer;
		Sprite blank;

		Dictionary<string, Sprite> sprites = new Dictionary<string,Sprite>();
		Viewport viewport;

		public Sidebar(Race race, Renderer renderer, Viewport viewport)
		{
			this.viewport = viewport;
			viewport.AddRegion( Region.Create(viewport, DockStyle.Right, 128, Paint));
			techTree.CurrentRace = race;
			techTree.Build("FACT", true);
			spriteRenderer = new SpriteRenderer(renderer, false);

			LoadSprites("buildings.txt");
			LoadSprites("units.txt");

			blank = SheetBuilder.Add(new Size(64, 48), 16);
		}

		void LoadSprites(string filename)
		{
			foreach (string line in Util.ReadAllLines(FileSystem.Open(filename)))
			{
				string key = line.Substring(0, line.IndexOf(','));
				sprites.Add(key, SpriteSheetBuilder.LoadSprite(key + "icon.shp"));
			}
		}

		void DrawSprite(Sprite s, ref float2 p)
		{
			spriteRenderer.DrawSprite(s, p, 0);
			p.Y += 48;
		}

		void Fill(float height, float2 p)
		{
			while (p.Y < height)
				DrawSprite(blank, ref p);
		}

		public void Paint()
		{
			float2 buildPos = viewport.Location + new float2(viewport.Size.X - 128, 0);
			float2 unitPos = viewport.Location + new float2(viewport.Size.X - 64, 0);
			
			foreach (Item i in techTree.BuildableItems)
			{
				Sprite sprite;
				if (!sprites.TryGetValue(i.tag, out sprite)) continue;

				if (i.IsStructure)
					DrawSprite( sprite, ref buildPos );
				else
					DrawSprite( sprite, ref unitPos );
			}

			Fill( viewport.Location.Y + viewport.Size.Y, buildPos );
			Fill( viewport.Location.Y + viewport.Size.Y, unitPos );

			spriteRenderer.Flush();
		}
	}
}
