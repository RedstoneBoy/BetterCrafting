﻿using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using static BetterCrafting.CategoryManager;

namespace BetterCrafting
{
    public class ModEntry : Mod
    {
        public static bool oldMenu = false;
        private CategoryData categoryData;
        public Nullable<ItemCategory> lastCategory;

        public override void Entry(IModHelper helper)
        {
            this.categoryData = this.Helper.Data.ReadJsonFile<CategoryData>("categories.json");
            if (this.categoryData == null)
            {
                this.Monitor.Log("Failed to read category data from 'categories.json'", LogLevel.Error);
                this.Monitor.Log("All crafting recipes will be in the 'Misc' tab", LogLevel.Warn);

                this.categoryData = new CategoryData();
            }

            MenuEvents.MenuChanged += MenuChanged;
            MenuEvents.MenuClosed += MenuClosed;
            PlayerEvents.InventoryChanged += InventoryChanged;
        }

        private void MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (e.NewMenu is GameMenu gameMenu && !oldMenu)
            {
                var craftingTabNum = gameMenu.getTabNumberFromName("crafting");
                var pages = this.Helper.Reflection.GetFieldValue<List<IClickableMenu>>(gameMenu, "pages");
                pages[craftingTabNum] = new BetterCraftingPage(this, this.categoryData, this.lastCategory);
            }
        }

        private void MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            oldMenu = false;
        }
        
        private void InventoryChanged(object sender, EventArgsInventoryChanged e)
        {
            if (Game1.activeClickableMenu is GameMenu gameMenu && !oldMenu)
            {
                var craftingTabNum = gameMenu.getTabNumberFromName("crafting");
                if (gameMenu.currentTab == craftingTabNum)
                {
                    var pages = this.Helper.Reflection.GetFieldValue<List<IClickableMenu>>(gameMenu, "pages");
                    ((BetterCraftingPage)pages[craftingTabNum]).UpdateInventory();
                }
            }
        }
    }
}