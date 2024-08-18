﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOSSE
{
    public partial class ShopEditingForm : Form
    {
        private const int MaxShopItem = 100;
        private const int MaxShopBlueprint = 150;
        private const int MaxShopRecipe = 100;
        private const int MaxShopPattern = 30;
        private const int MaxShopAnimal = 30;
        private struct Offsets
        {
            public int Item { get; set; }
            public int Blueprint { get; set; }
            public int Recipe { get; set; }
            public int Pattern { get; set; }
            public int Animal { get; set; }
        }
        private static readonly Offsets[] offset = new Offsets[] { 
            new Offsets { Item = 0x4E638, Blueprint = 0x4E890, Recipe = 0x4E892, Pattern = 0x4E8AE, Animal = 0x4E8AE },
            new Offsets { Item = 0x4E8B4, Blueprint = 0x4EB0C, Recipe = 0x4EC5E, Pattern = 0x4EC5E, Animal = 0x4EC5E },
            new Offsets { Item = 0x4EC64, Blueprint = 0x4EEBC, Recipe = 0x4EEEE, Pattern = 0x4EEEE, Animal = 0x4EF1E },
            new Offsets { Item = 0x4EF24, Blueprint = 0x4F17C, Recipe = 0x4F212, Pattern = 0x4F276, Animal = 0x4F294 },
            new Offsets { Item = 0x4F2B8, Blueprint = 0x4F510, Recipe = 0x4F5A6, Pattern = 0x4F60A, Animal = 0x4F628 },
            new Offsets { Item = 0x4F64C, Blueprint = 0x4F8A4, Recipe = 0x4F93A, Pattern = 0x4F99E, Animal = 0x4F9BC },
            new Offsets { Item = 0x4F9E0, Blueprint = 0x4FC38, Recipe = 0x4FCCE, Pattern = 0x4FD32, Animal = 0x4FC50 },
            new Offsets { Item = 0x4FD74, Blueprint = 0x4FFCC, Recipe = 0x50062, Pattern = 0x500C6, Animal = 0x500E4 },
            new Offsets { Item = 0x50108, Blueprint = 0x50360, Recipe = 0x503F6, Pattern = 0x5045A, Animal = 0x50478 },
            new Offsets { Item = 0x5049C, Blueprint = 0x506F4, Recipe = 0x5078A, Pattern = 0x507EE, Animal = 0x5080C },
        };
        private struct ShopCategory
        {
            public const int Item = 0;
            public const int Blueprint = 1;
            public const int Recipe = 2;
            public const int Pattern = 3;
            public const int Animal = 4;
        }
        private bool[] isResourceLoaded = { true, false, false, false, false };
        private int[] itemCount = { 32, 5, 8, 0, 68, 29, 39, 29, 65, 28, 7 };
        private int currentShop;
        #region Shop data
        private int[][] blueprint = 
        {
            new int[] {}, //{0x231, 0x235},
            new int[] {0x2B2, 0x2B3, 0x2B4, 0x2B5, 0x2B6, 0x2B7, 0x2B8, 0x2B9, 0x2BA, 0x2BB, 0x2BC, 0x2BD, 0x2BE},
            new int[] {0x15B, 0x15C, 0x15E, 0x162, 0x169, 0x175, 0x18B, 0x18D, 0x192, 0x195, 0x196, 0x199, 0x19D, 0x1AB, 0x1BD, 0x1C3, 0x1D3, 0x1DD, 0x1E1, 0x1EC, 0x1F1, 0x1F8, 0x202, 0x20A, 0x215, 0x216, 0x225, 0x226, 0x22D, 0x230, 0x231, 0x235},
            new int[] {0x0, 0x1, 0x3, 0x11, 0x25, 0x26, 0x37, 0x39, 0x3F, 0x43, 0x45, 0x46, 0x59, 0x5F, 0x61, 0x6E, 0x70, 0x75, 0x82, 0x88, 0x8B, 0x9C, 0x9F, 0xA6, 0xB0, 0xBD, 0xC4, 0xC7, 0xCE, 0xD1, 0xD8, 0xE8, 0xEB, 0xF7, 0x107, 0x111, 0x113, 0x13A, 0x13B, 0x16, 0x17, 0x18, 0x147, 0x154, 0x157, 0x15A, 0x161, 0x165, 0x168, 0x16C, 0x170, 0x176, 0x181, 0x184, 0x188, 0x1A1, 0x1A2, 0x1A6, 0x1B3, 0x1BC, 0x1C2, 0x1C8, 0x1CE, 0x1D4, 0x1DA, 0x1E6, 0x1F2, 0x1FE, 0x204, 0x205, 0x209, 0x20F, 0x210, 0x21A, 0x21E, 0x224, 0x23A, 0x23B, 0x23C, 0x240, 0x241, 0x242, 0x243, 0x244, 0x245, 0x246, 0x25C, 0x25D, 0x25E, 0x25F, 0x260, 0x261, 0x262, 0x263, 0x264, 0x26B, 0x26C, 0x26D, 0x273, 0x277, 0x278, 0x279, 0x27A, 0x27B, 0x27C, 0x27D, 0x29C, 0x29D, 0x29E, 0x29F, 0x2A0, 0x2A1, 0x2A2, 0x2AA, 0x2C4, 0x2C8, 0x2C9, 0x2CA, 0x2CD, 0x2D0, 0x2D3, 0x2D6, 0x2DA, 0x2DB, 0x2DE, 0x2DF, 0x2E8, 0x2E9, 0x32F, 0x330, 0x331, 0x333},
            new int[] {0x4, 0x24, 0x27, 0x31, 0x34, 0x49, 0x4A, 0x4E, 0x4F, 0x50, 0x55, 0x56, 0x5E, 0x6B, 0x6C, 0x71, 0x96, 0xA0, 0xAA, 0xB4, 0xBC, 0xBE, 0xDE, 0xDF, 0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xF8, 0x108, 0x10A, 0x117, 0x11A, 0x11B, 0x11F, 0x133, 0x134, 0x135, 0x1C, 0x1D, 0x1E, 0x143, 0x14D, 0x151, 0x156, 0x15D, 0x164, 0x16B, 0x172, 0x179, 0x180, 0x187, 0x18E, 0x1A3, 0x1AD, 0x1B4, 0x1C9, 0x1CD, 0x1DB, 0x1DF, 0x1E0, 0x1E7, 0x1F3, 0x1FF, 0x20B, 0x219, 0x223, 0x22F, 0x23F, 0x275, 0x2A5, 0x2C3, 0x2CF, 0x2D5, 0x2E0, 0x2EA, 0x2F2, 0x2FC, 0x306, 0x310, 0x313, 0x314, 0x31A, 0x31D, 0x31E, 0x324, 0x327, 0x328, 0x332},
            new int[] {0x2, 0x7, 0x10, 0x2A, 0x32, 0x40, 0x41, 0x48, 0x4B, 0x51, 0x58, 0x64, 0x66, 0x68, 0x79, 0x8F, 0x99, 0xAD, 0xB7, 0xC1, 0xCF, 0xD4, 0xD5, 0xE7, 0xF0, 0xF2, 0xF9, 0x102, 0x105, 0x109, 0x10C, 0x10E, 0x10F, 0x118, 0x11C, 0x121, 0x122, 0x123, 0x124, 0x125, 0x13C, 0x13D, 0x19, 0x1A, 0x1B, 0x145, 0x14F, 0x173, 0x17A, 0x185, 0x197, 0x1A4, 0x1A9, 0x1B2, 0x1B7, 0x1B8, 0x1BF, 0x1C5, 0x1CB, 0x1D1, 0x1D7, 0x1E3, 0x1E9, 0x1EF, 0x1F5, 0x1FB, 0x201, 0x207, 0x21C, 0x229, 0x22A, 0x23E, 0x271, 0x2A4, 0x2A6, 0x2C0, 0x2C5, 0x2CB, 0x2D1, 0x2DC},
            new int[] {0x6, 0xC, 0x29, 0x30, 0x33, 0x35, 0x57, 0x62, 0x65, 0x67, 0x69, 0x72, 0x73, 0x77, 0x87, 0x8A, 0x8E, 0x94, 0xA2, 0xAC, 0xB6, 0xC0, 0xC6, 0xE6, 0xFB, 0x10B, 0x11D, 0x1F, 0x20, 0x21, 0x142, 0x159, 0x160, 0x167, 0x16E, 0x17C, 0x183, 0x18A, 0x190, 0x1AC, 0x1B1, 0x1D5, 0x1ED, 0x1F9, 0x203, 0x213, 0x217, 0x222, 0x22E, 0x24D, 0x24E, 0x24F, 0x250, 0x251, 0x252, 0x253, 0x254, 0x27E, 0x27F, 0x280, 0x281, 0x282, 0x283, 0x2AC, 0x2CE, 0x2D4, 0x2D7, 0x2E1, 0x2EB, 0x2F5, 0x2FF, 0x309},
            new int[] {0x8, 0x12, 0x2B, 0x36, 0x3A, 0x3B, 0x3D, 0x3E, 0x44, 0x47, 0x5A, 0x5B, 0x60, 0x76, 0x90, 0x9A, 0xAE, 0xB2, 0xC2, 0xC8, 0xD0, 0xDC, 0xEC, 0xEE, 0xEF, 0xF1, 0xFC, 0xFD, 0x101, 0x114, 0x115, 0x119, 0x136, 0x137, 0x138, 0x139, 0x13, 0x14, 0x15, 0x146, 0x14C, 0x150, 0x177, 0x17E, 0x18C, 0x191, 0x193, 0x194, 0x198, 0x1A8, 0x1AE, 0x1B6, 0x1BE, 0x1CA, 0x1D6, 0x1E2, 0x1E5, 0x1EB, 0x1EE, 0x1FA, 0x206, 0x20D, 0x21D, 0x21F, 0x228, 0x22B, 0x232, 0x247, 0x248, 0x249, 0x24A, 0x24B, 0x24C, 0x26E, 0x26F, 0x270, 0x276, 0x284, 0x285, 0x286, 0x287, 0x288, 0x289, 0x28A, 0x28B, 0x2A3, 0x2C1, 0x2C6, 0x2CC, 0x2D2, 0x2E2, 0x2EC, 0x2F6, 0x300, 0x30A},
            new int[] {0x5, 0x28, 0x42, 0x4C, 0x4D, 0x52, 0x53, 0x6A, 0x6D, 0x74, 0x8D, 0x97, 0x9E, 0xA1, 0xB5, 0xBF, 0xD6, 0xD7, 0xDA, 0xE9, 0xF3, 0xF4, 0x103, 0x104, 0x110, 0x126, 0x12D, 0x12E, 0x12F, 0x144, 0x14E, 0x155, 0x163, 0x16A, 0x171, 0x178, 0x17F, 0x186, 0x19A, 0x19B, 0x19C, 0x1AA, 0x1C0, 0x1C4, 0x1CC, 0x1D0, 0x1D8, 0x1DC, 0x1E4, 0x1F0, 0x1F4, 0x200, 0x208, 0x20C, 0x221, 0x227, 0x255, 0x256, 0x257, 0x258, 0x259, 0x25A, 0x25B, 0x272, 0x2AE, 0x2B0, 0x293, 0x294, 0x295, 0x296, 0x297, 0x298, 0x299, 0x29A, 0x29B, 0x2A7, 0x2A8, 0x2C2, 0x2C7, 0x2E5, 0x2EF, 0x32E},
            new int[] {0x9, 0xB, 0xF, 0x2C, 0x2E, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0x80, 0x81, 0x83, 0x86, 0x91, 0x93, 0x9B, 0xA4, 0xA5, 0xA7, 0xB1, 0xB9, 0xBB, 0xC3, 0xC5, 0xEA, 0xF6, 0x106, 0x11E, 0x120, 0x22, 0x23, 0x140, 0x14A, 0x158, 0x15F, 0x166, 0x16D, 0x174, 0x17B, 0x182, 0x189, 0x18F, 0x19E, 0x19F, 0x1B0, 0x1B5, 0x1C6, 0x1D2, 0x1DE, 0x1F6, 0x1FC, 0x20E, 0x211, 0x214, 0x21B, 0x22C, 0x2AF, 0x28C, 0x28D, 0x28E, 0x28F, 0x290, 0x291, 0x292, 0x2A9, 0x2E6, 0x2F0, 0x2FA, 0x304, 0x30E, 0x318, 0x322, 0x32C}
        };
        private int[][] recipe =
        {
            new int[] {}, //{0x2B, 0x2E, 0x31, 0x37},
            new int[] {},
            new int[] {},
            new int[] {0x1, 0x2, 0x5, 0x28, 0x2B, 0x2E, 0x31, 0x37, 0x3A, 0x46, 0x53, 0x58, 0x59, 0x5A, 0x5F, 0x62, 0x65, 0x74, 0x75, 0x77, 0x80, 0x85, 0x87, 0x8A, 0x91, 0x95, 0xAA, 0xAB, 0xAE, 0xB2, 0xBF, 0xC0, 0x114, 0x129, 0x12A, 0x134, 0x139, 0x145, 0x14E, 0x157, 0x163, 0x16E, 0x171, 0x17C, 0x18A},
            new int[] {0x6, 0x32, 0x33, 0x4C, 0x4E, 0x4F, 0x73, 0x76, 0x86, 0x99, 0xB0, 0xD9, 0xE0, 0xE1, 0xF2, 0x14C, 0x162},
            new int[] {0x3, 0x4, 0x2D, 0x2F, 0x3D, 0x45, 0x5C, 0x5D, 0x66, 0x71, 0x7A, 0x82, 0x8B, 0x90, 0x92, 0x94, 0xA2, 0xA7, 0xA8, 0xAC, 0xAF, 0xB3, 0xB7, 0xBA, 0xC1, 0xC2, 0xC4, 0xC7, 0xC8, 0xCA, 0xD0, 0xD1, 0xD3, 0xD4, 0xD5, 0xD6, 0xE4, 0xE7, 0xEE, 0xEF, 0xFE, 0x105, 0x107, 0x117, 0x131, 0x137, 0x13F, 0x14A, 0x152, 0x164, 0x16A, 0x16C, 0x17D, 0x181},
            new int[] {0x27, 0x2C, 0x4A, 0x56, 0x5E, 0x68, 0x69, 0x6C, 0x6E, 0x72, 0x7F, 0x8D, 0x8F, 0x9D, 0x9F, 0xDA, 0xDB, 0xE2, 0xE6, 0xE9, 0xF4, 0xF8, 0x104, 0x108, 0x111, 0x121, 0x128, 0x12C, 0x12F, 0x132, 0x138, 0x146, 0x15B, 0x15C, 0x165, 0x167, 0x16D, 0x183, 0x184},
            new int[] {0x34, 0x36, 0x38, 0x3F, 0x41, 0x43, 0x47, 0x52, 0x54, 0x55, 0x57, 0x5B, 0x63, 0x67, 0x79, 0x7C, 0x7D, 0x7E, 0x88, 0x96, 0x97, 0x9A, 0x9C, 0x9E, 0xA0, 0xA3, 0xA4, 0xA5, 0xB8, 0xC5, 0xCE, 0xDF, 0xE3, 0xE8, 0xEA, 0xF0, 0xF1, 0xF3, 0xF6, 0xF7, 0xFA, 0x101, 0x102, 0x106, 0x10F, 0x110, 0x113, 0x115, 0x118, 0x11D, 0x126, 0x12D, 0x12E, 0x135, 0x15A, 0x15E, 0x166, 0x16B, 0x16F, 0x175, 0x176, 0x179, 0x17E, 0x189},
            new int[] {0x40, 0x89, 0x10D, 0x119, 0x11A, 0x11F, 0x13B, 0x13C, 0x141, 0x142, 0x155, 0x156, 0x158, 0x159, 0x17A, 0x17B, 0x17F, 0x180},
            new int[] {0x59, 0x5A},
        };
        private int[][] pattern =
        {
            new int[] {},
            new int[] {},
            new int[] {0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x64, 0x7D, 0x7E, 0x7F, 0x80, 0x81, 0x97, 0x99, 0x9B},
            new int[] {0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x79, 0x89, 0x90},
            new int[] {0x58, 0x76, 0x8A, 0x91},
            new int[] {0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x59, 0x66, 0x69, 0x92},
            new int[] {0x5B, 0x5D, 0x61, 0x65, 0x68, 0x93, 0x98},
            new int[] {0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40, 0x41, 0x42, 0x43, 0x5C, 0x5E, 0x60, 0x63, 0x75, 0x78, 0x7A, 0x94, 0x9A},
            new int[] {0x44, 0x8B, 0x8C, 0x95},
            new int[] {0x45, 0x77, 0x96},
        };
        private int[][] animal =
        {
            new int[] {},
            new int[] {},
            new int[] {},
            new int[] {0x2, 0x20, 0x38, 0x3A, 0x34, 0x36, 0x52, 0x58, 0x5A, 0x5C, 0x7B, 0x7C, 0x7D, 0x7E},
            new int[] {0x2, 0x10, 0x18, 0x1C, 0x20, 0x24, 0x3E, 0x2C, 0x30},
            new int[] {0x40, 0x42, 0x44, 0x5E},
            new int[] {0x3C, 0x28, 0x50, 0x54, 0x56},
            new int[] {0x46, 0x48, 0x4A, 0x4C, 0x4E},
            new int[] {0x46, 0x4E},
            new int[] {0x3C, 0x28},
        };
        #endregion

        public bool DataLoaded { get; private set; }
        public bool IsModified { get; private set; }
        public static string[] ShopNameList;

        public ShopEditingForm()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
            DataLoaded = false;
            IsModified = false;
            Util.LoadItemNameList();
            Util.LoadShopNameList();
            
            shopComboBox.Items.AddRange(ShopNameList);

            itemColumn.Items.Add("None");
            for (int i = 0; i < Item.MaxItem; i++)
                itemColumn.Items.Add(Item.ItemNameList[ItemEditingForm.ItemSortedList[i]]);

            // Row must be added after adding ComboBox items to avoid bad performance
            itemDataGridView.Rows.Add(MaxShopItem);

            shopComboBox.SelectedIndex = 0;
            //implicitly call shopComboBox_SelectedIndexChanged event
        }

        // Load
        private void displayShopData(int shopIndex)
        {
            DataLoaded = false;
            if (isResourceLoaded[ShopCategory.Item]) displayItem(shopIndex);
            if (isResourceLoaded[ShopCategory.Blueprint]) displayBlueprint(shopIndex);
            if (isResourceLoaded[ShopCategory.Recipe]) displayRecipe(shopIndex);
            if (isResourceLoaded[ShopCategory.Pattern]) displayPattern(shopIndex);
            if (isResourceLoaded[ShopCategory.Animal]) displayAnimal(shopIndex);
            DataLoaded = true;
        }

        private void displayItem(int shopIndex)
        {
            int localoffset = offset[shopIndex].Item;
            for (int i = 0; i < MaxShopItem; i++)
            {
                var row = itemDataGridView.Rows[i];

                short itemIndex = BitConverter.ToInt16(MainForm.SaveData,
                    localoffset);
                if (itemIndex == -1)
                {
                    row.Cells[0].Value = "None";
                    row.Cells[1].ReadOnly = true;
                    row.Cells[1].Style = MainForm.GrayCellStyle;
                }
                else
                {
                    row.Cells[0].Value = Item.ItemNameList[itemIndex];
                    row.Cells[1].ReadOnly = false;
                    row.Cells[1].Style = itemAvailabilityColumn.DefaultCellStyle;
                }

                // Prevent changes to the game's original shop items.
                if (i < itemCount[shopIndex])
                {
                    row.Cells[0].ReadOnly = true;
                    row.Cells[0].Style = MainForm.GrayCellStyle;
                }
                else
                {
                    row.Cells[0].ReadOnly = false;
                    row.Cells[0].Style = itemColumn.DefaultCellStyle;
                }

                bool avail = (MainForm.SaveData[localoffset + 0x4] & 0x1) == 0x1;
                row.Cells[1].Value = avail;

                row.Cells[2].Value = BitConverter.ToInt16(MainForm.SaveData,
                    localoffset + 0x2);
                if ((itemIndex == -1) || (!avail))
                {
                    row.Cells[2].ReadOnly = true;
                    row.Cells[2].Style = MainForm.GrayCellStyle;
                }
                else
                {
                    row.Cells[2].ReadOnly = false;
                    row.Cells[2].Style = itemStockColumn.DefaultCellStyle;
                }

                localoffset += 0x6;
            }
        }

        private void displayBlueprint(int shopIndex)
        {
            int localoffset = offset[shopIndex].Blueprint;
            for (int i = 0; i < blueprint[shopIndex].Length; i++)
            {
                var row = blueprintDataGridView.Rows[i];
                row.Cells[0].Value =
                    BlueprintEditingForm.BlueprintSetNameList[blueprint[shopIndex][i]];
                row.Cells[1].Value = (MainForm.SaveData[localoffset] & 0x1) == 0x1;
                row.Cells[2].Value = (MainForm.SaveData[localoffset] & 0x2) == 0x2;
                localoffset++;
            }
            for (int i = blueprint[shopIndex].Length; i < MaxShopBlueprint; i++)
            {
                var row = blueprintDataGridView.Rows[i];
                row.Cells[0].Value = row.Cells[1].Value = row.Cells[2].Value = null;
            }
        }

        private void displayRecipe(int shopIndex)
        {
            int localoffset = offset[shopIndex].Recipe;
            for (int i = 0; i < recipe[shopIndex].Length; i++)
            {
                var row = recipeDataGridView.Rows[i];
                row.Cells[0].Value =
                    RecipeEditingForm.RecipeSetNameList[recipe[shopIndex][i]];
                row.Cells[1].Value = (MainForm.SaveData[localoffset] & 0x1) == 0x1;
                row.Cells[2].Value = (MainForm.SaveData[localoffset] & 0x2) == 0x2;
                localoffset++;
            }
            for (int i = recipe[shopIndex].Length; i < MaxShopRecipe; i++)
            {
                var row = recipeDataGridView.Rows[i];
                row.Cells[0].Value = row.Cells[1].Value = row.Cells[2].Value = null;
            }
        }

        private void displayPattern(int shopIndex)
        {
            int localoffset = offset[shopIndex].Pattern;
            for (int i = 0; i < pattern[shopIndex].Length; i++)
            {
                var row = patternDataGridView.Rows[i];
                row.Cells[0].Value =
                    PatternEditingForm.PatternSetNameList[pattern[shopIndex][i]];
                row.Cells[1].Value = (MainForm.SaveData[localoffset] & 0x1) == 0x1;
                row.Cells[2].Value = (MainForm.SaveData[localoffset] & 0x2) == 0x2;
                localoffset++;
            }
            for (int i = pattern[shopIndex].Length; i < MaxShopPattern; i++)
            {
                var row = patternDataGridView.Rows[i];
                row.Cells[0].Value = row.Cells[1].Value = row.Cells[2].Value = null;
            }
        }

        private void displayAnimal(int shopIndex)
        {
            int localoffset = offset[shopIndex].Animal;
            for (int i = 0; i < animal[shopIndex].Length; i++)
            {
                var row = animalDataGridView.Rows[i];
                row.Cells[0].Value =
                    AnimalEditingForm.AnimalTypeList[animal[shopIndex][i]];
                row.Cells[1].Value =
                    (MainForm.SaveData[localoffset] & 0x1) == 0x1;
                localoffset++;
            }
            for (int i = animal[shopIndex].Length; i < MaxShopAnimal; i++)
            {
                var row = animalDataGridView.Rows[i];
                row.Cells[0].Value = row.Cells[1].Value = null;
            }
        }
        
        // Save
        private void saveCurrentShop()
        {
            if (isResourceLoaded[ShopCategory.Item]) saveItem(currentShop);
            if (isResourceLoaded[ShopCategory.Blueprint]) saveBlueprint(currentShop);
            if (isResourceLoaded[ShopCategory.Recipe]) saveRecipe(currentShop);
            if (isResourceLoaded[ShopCategory.Pattern]) savePattern(currentShop);
            if (isResourceLoaded[ShopCategory.Animal]) saveAnimal(currentShop);
        }

        private void saveItem(int shopIndex)
        {
            int localoffset = offset[currentShop].Item;
            for (int i = 0; i < MaxShopItem; i++)
            {
                var row = itemDataGridView.Rows[i];

                int itemIndex = Array.IndexOf(Item.ItemNameList,
                    row.Cells[0].Value.ToString());
                MainForm.SaveData[localoffset] = (byte)(itemIndex & 0xFF);
                MainForm.SaveData[localoffset + 0x1] = (byte)((itemIndex >> 8) & 0xFF);

                bool itemAvailable = (bool)row.Cells[1].Value;
                if (itemAvailable)
                    MainForm.SaveData[localoffset + 0x4] |= 0x1;
                else
                    MainForm.SaveData[localoffset + 0x4] &= 0xFE;

                // Error shouldn't happen because of CellValidating.
                ushort stock = UInt16.Parse(row.Cells[2].Value.ToString());
                MainForm.SaveData[localoffset + 0x2] = (byte)(stock & 0xFF);
                MainForm.SaveData[localoffset + 0x3] = (byte)((stock >> 8) & 0xFF);

                localoffset += 6;
            }
        }

        private void saveBlueprint(int shopIndex)
        {
            int localoffset = offset[currentShop].Blueprint;
            for (int i = 0; i < blueprint[currentShop].Length; i++)
            {
                var row = blueprintDataGridView.Rows[i];
                if ((bool)row.Cells[1].Value)
                    MainForm.SaveData[localoffset] |= 0x1;
                else
                    MainForm.SaveData[localoffset] &= 0xFE;
                if ((bool)row.Cells[2].Value)
                    MainForm.SaveData[localoffset] |= 0x2;
                else
                    MainForm.SaveData[localoffset] &= 0xFD;
                localoffset++;
            }
        }

        private void saveRecipe(int shopIndex)
        {
            int localoffset = offset[currentShop].Recipe;
            for (int i = 0; i < recipe[currentShop].Length; i++)
            {
                var row = recipeDataGridView.Rows[i];
                if ((bool)row.Cells[1].Value)
                    MainForm.SaveData[localoffset] |= 0x1;
                else
                    MainForm.SaveData[localoffset] &= 0xFE;
                if ((bool)row.Cells[2].Value)
                    MainForm.SaveData[localoffset] |= 0x2;
                else
                    MainForm.SaveData[localoffset] &= 0xFD;
                localoffset++;
            }
        }

        private void savePattern(int shopIndex)
        {
            int localoffset = offset[currentShop].Pattern;
            for (int i = 0; i < pattern[currentShop].Length; i++)
            {
                var row = patternDataGridView.Rows[i];
                if ((bool)row.Cells[1].Value)
                    MainForm.SaveData[localoffset] |= 0x1;
                else
                    MainForm.SaveData[localoffset] &= 0xFE;
                if ((bool)row.Cells[2].Value)
                    MainForm.SaveData[localoffset] |= 0x2;
                else
                    MainForm.SaveData[localoffset] &= 0xFD;
                localoffset++;
            }
        }

        private void saveAnimal(int shopIndex)
        {
            int localoffset = offset[currentShop].Animal;
            for (int i = 0; i < animal[currentShop].Length; i++)
            {
                var row = animalDataGridView.Rows[i];
                if ((bool)row.Cells[1].Value)
                    MainForm.SaveData[localoffset] |= 0x1;
                else
                    MainForm.SaveData[localoffset] &= 0xFE;
                localoffset++;
            }
        }

        private void shopComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DataLoaded) saveCurrentShop();
            currentShop = shopComboBox.SelectedIndex;
            displayShopData(currentShop);
        }

        // Drop a list when user click to a cell
        private void itemDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0) return;
            var cb = (ComboBox)itemDataGridView.EditingControl;
            if (cb != null) cb.DroppedDown = true;
        }

        // Commit a change after user change item in ComboBox
        private void itemDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            int columnIndex = itemDataGridView.CurrentCell.ColumnIndex;
            if (DataLoaded && (columnIndex == 0 || columnIndex == 1))
                itemDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        // Change cells based on item
        private void itemDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!DataLoaded) return;
            var row = itemDataGridView.Rows[e.RowIndex];
            if (e.ColumnIndex == 0)
            {
                int itemIndex = Array.IndexOf(Item.ItemNameList,
                    row.Cells[0].Value.ToString());
                if (itemIndex == -1)
                {
                    row.Cells[1].ReadOnly = true;
                    row.Cells[1].Style = MainForm.GrayCellStyle;
                }
                else
                {
                    row.Cells[1].ReadOnly = false;
                    row.Cells[1].Style = itemAvailabilityColumn.DefaultCellStyle;
                }
                if ((itemIndex == -1) || (!(bool)row.Cells[1].Value))
                {
                    row.Cells[2].ReadOnly = true;
                    row.Cells[2].Style = MainForm.GrayCellStyle;
                }
                else
                {
                    row.Cells[2].ReadOnly = false;
                    row.Cells[2].Style = itemStockColumn.DefaultCellStyle;
                }
            }
            if (e.ColumnIndex == 1)
            {
                if (!(bool)row.Cells[1].Value)
                {
                    row.Cells[2].ReadOnly = true;
                    row.Cells[2].Style = MainForm.GrayCellStyle;
                }
                else
                {
                    row.Cells[2].ReadOnly = false;
                    row.Cells[2].Style = itemStockColumn.DefaultCellStyle;
                }
            }
        }

        // Validate input value
        private void itemDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!DataLoaded) return;
            if (e.ColumnIndex == 2)
            {
                var cell = itemDataGridView.Rows[e.RowIndex].Cells[2];
                ushort stock;
                bool isValid = UInt16.TryParse(e.FormattedValue.ToString(), out stock);
                if (!isValid)
                {
                    cell.ErrorText = "Invalid number";
                    itemDataGridView.CancelEdit();
                }
                else
                    cell.ErrorText = null;
            }
        }

        // Load resource when user change tab
        private void shopTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (shopTabControl.SelectedIndex)
            {
                // Item resource was loaded by default
                case ShopCategory.Blueprint:
                    if (!isResourceLoaded[ShopCategory.Blueprint])
                    {
                        Util.LoadBlueprintSetNameList();
                        isResourceLoaded[ShopCategory.Blueprint] = true;
                        blueprintDataGridView.Rows.Add(MaxShopBlueprint);
                        displayBlueprint(currentShop);
                    }
                    break;
                case ShopCategory.Recipe:
                    if (!isResourceLoaded[ShopCategory.Recipe])
                    {
                        Util.LoadRecipeSetNameList();
                        isResourceLoaded[ShopCategory.Recipe] = true;
                        recipeDataGridView.Rows.Add(MaxShopRecipe);
                        displayRecipe(currentShop);
                    }
                    break;
                case ShopCategory.Pattern:
                    if (!isResourceLoaded[ShopCategory.Pattern])
                    {
                        Util.LoadPatternSetNameList();
                        isResourceLoaded[ShopCategory.Pattern] = true;
                        patternDataGridView.Rows.Add(MaxShopPattern);
                        displayPattern(currentShop);
                    }
                    break;
                case ShopCategory.Animal:
                    if (!isResourceLoaded[ShopCategory.Animal])
                    {
                        Util.LoadAnimalTypeList();
                        isResourceLoaded[ShopCategory.Animal] = true;
                        animalDataGridView.Rows.Add(MaxShopAnimal);
                        displayAnimal(currentShop);
                    }
                    break;
            }
        }

        private void ShopEditingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveCurrentShop();
        }
    }
}
