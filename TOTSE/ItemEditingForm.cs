﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SOSSE
{
    public partial class ItemEditingForm : Form
    {
        private const int MaxBagItem = 100;
        private const int MaxContainerItem = 999;

        private class ItemContainer
        {
            public int Offset { get; set; }
            public int Count { get; set; }
            public Item[] Items { get; set; }
            public DataGridView ContainerDataGridView { get; set; }
            public TabPage Tab { get; set; }
        }
        private ItemContainer[] containers;
        private enum ContainerID { Bag, Toolbox, Fridge, Storage, Material, Wardrobe };

        public bool DataLoaded { get; private set; }
        public bool IsModified { get; private set; }

        public static readonly string[] StarQualityList = { "--", "0", "0.5", "1",
            "1.5", "2", "2.5", "3", "3.5", "4", "4.5", "5", "5+" };
        #region public static readonly ushort[] ItemSortedList
        public static readonly ushort[] ItemSortedList = { 0x178, 0x74A, 0x746,
            0x669, 0xF5, 0x31A, 0x260, 0x256, 0x8D, 0x6EF, 0x28E, 0x259, 0x116,
            0x117, 0x326, 0x327, 0x6C1, 0x533, 0x379, 0x37A, 0x744, 0x75A, 0xD2,
            0x6A, 0x66B, 0x1CD, 0x702, 0x705, 0x33, 0x37, 0xF6, 0x26D, 0x312,
            0x485, 0x231, 0x8E, 0x2B0, 0x29B, 0x48F, 0x6F9, 0x24F, 0x505, 0x511,
            0x530, 0x532, 0x1E5, 0x124, 0x2F7, 0x2F8, 0x2F1, 0x2F2, 0x66A,
            0x71C, 0x100, 0x717, 0x16, 0x698, 0x70C, 0x1B1, 0x38, 0x186, 0x23E,
            0x3D, 0x72C, 0x9A, 0x6FA, 0xF3, 0x293, 0x26B, 0x310, 0x287, 0x483,
            0x8B, 0x2AE, 0x48D, 0x2F, 0x6F0, 0x206, 0x662, 0x25F, 0x686, 0x5A2,
            0x46D, 0x468, 0x472, 0x477, 0x12B, 0x31, 0xD0, 0x68, 0x6CD, 0x652,
            0x5CC, 0x6F2, 0x191, 0x444, 0x47A, 0x45F, 0x3E7, 0x3F1, 0x59C,
            0x5E4, 0x3F9, 0x3F6, 0x40D, 0x5E3, 0x77E, 0x3EE, 0x3E4, 0x5E6,
            0x40A, 0x5E1, 0x77B, 0x517, 0x525, 0x36C, 0x376, 0x646, 0x50C,
            0x728, 0x5C7, 0x437, 0x432, 0x43C, 0x441, 0x3BB, 0x3C5, 0x41D,
            0x3A2, 0x5C2, 0x58A, 0x5EE, 0x58C, 0x541, 0x427, 0x3AC, 0x37B,
            0x37E, 0x732, 0x598, 0x5B2, 0x566, 0x388, 0x392, 0x403, 0x7A1,
            0x72F, 0x57F, 0x519, 0x735, 0x6A4, 0x3CF, 0x3D9, 0x354, 0x35E,
            0x5B4, 0x340, 0x34A, 0x5DB, 0x71F, 0x369, 0x373, 0x59E, 0x3B8,
            0x3C2, 0x41A, 0x39F, 0x5C1, 0x6B8, 0x56C, 0x764, 0x719, 0x668,
            0x534, 0x424, 0x3A9, 0x75D, 0x792, 0x59A, 0x5BC, 0x5AF, 0x58F,
            0x753, 0x385, 0x38F, 0x400, 0x79E, 0xE8, 0x80, 0x5A7, 0x3CC, 0x3D6,
            0x351, 0x35B, 0x5B7, 0x502, 0x513, 0x33D, 0x347, 0x549, 0x6F7,
            0x272, 0x316, 0x233, 0x2B4, 0x29C, 0x5DA, 0x62F, 0x789, 0x6E2,
            0x78A, 0x580, 0x1BC, 0x1AA, 0x1B3, 0x1B0, 0x1A8, 0x1AD, 0x1AB,
            0x64D, 0x770, 0x16A, 0x76D, 0x163, 0x168, 0x54A, 0x557, 0x526,
            0x506, 0x54F, 0x4FC, 0x551, 0x3F5, 0x3F4, 0x51D, 0x727, 0x1FD,
            0x301, 0x73B, 0x46B, 0x466, 0x470, 0x475, 0x581, 0x250, 0x276, 0xD8,
            0x70, 0x118, 0x119, 0x328, 0x329, 0x5BF, 0x6D0, 0x5C9, 0x547, 0x6C8,
            0x2A, 0x57C, 0xC8, 0x13D, 0x171, 0x60, 0x6B6, 0x2E1, 0x2E2, 0x205,
            0x29D, 0x195, 0x1EA, 0x18C, 0xB0, 0x1A7, 0x48, 0x5B8, 0xF8, 0x90,
            0xE9, 0x81, 0x296, 0x6FE, 0x120, 0x121, 0x330, 0x331, 0x5F9, 0x600,
            0x61E, 0x609, 0x610, 0x617, 0x228, 0x6A3, 0x20F, 0x23D, 0x1BB,
            0x295, 0x14F, 0x218, 0x569, 0xDD, 0x75, 0x64B, 0xCA, 0x283, 0x62,
            0x15D, 0x537, 0x577, 0x561, 0x710, 0x5A0, 0x703, 0x675, 0x688,
            0x443, 0x6EB, 0x643, 0x682, 0x52D, 0x2C0, 0x2C1, 0x290, 0x1B4,
            0x217, 0x160, 0x257, 0x5CD, 0xF1, 0x72A, 0x26A, 0x30F, 0x481, 0x230,
            0x89, 0x2AD, 0x29A, 0x642, 0x48B, 0x6F6, 0x1E3, 0x706, 0x248, 0xF0,
            0x88, 0x1F6, 0x24E, 0x254, 0x28F, 0x22A, 0x26F, 0x25D, 0x2B5, 0x314,
            0x229, 0x18D, 0x6E9, 0x23A, 0x174, 0x142, 0x177, 0x713, 0x25, 0x6B5,
            0x70D, 0x6A5, 0x699, 0x55F, 0x302, 0x6FB, 0x220, 0x6E6, 0x756,
            0x20A, 0x14D, 0x12C, 0x6B7, 0x695, 0x6C4, 0xA1, 0x108, 0x64C, 0x5B9,
            0x798, 0x799, 0x787, 0x56F, 0x568, 0x582, 0x563, 0x57B, 0x737, 0x2,
            0xA, 0x436, 0x431, 0x43B, 0x440, 0x76A, 0xA0, 0x107, 0x173, 0xBE,
            0x23B, 0x20E, 0x15B, 0x138, 0x56, 0x1BA, 0x1FF, 0x545, 0x101, 0x97,
            0x332, 0x597, 0x5C3, 0x57E, 0x1C3, 0x215, 0x251, 0x571, 0x22B,
            0x518, 0x61B, 0x606, 0x60D, 0x614, 0x5F6, 0x5FD, 0x1C1, 0x5D4,
            0x645, 0x73E, 0xB2, 0x134, 0x1F5, 0x4A, 0x1E9, 0x6E3, 0x1EB, 0x150,
            0x61C, 0x607, 0x60E, 0x615, 0x5F7, 0x5FE, 0x6B2, 0x54C, 0x46A,
            0x465, 0x46F, 0x474, 0x1C8, 0x69B, 0x743, 0x6CF, 0x70F, 0x5A1,
            0x704, 0x1E0, 0x690, 0x1DA, 0x539, 0x560, 0x5A6, 0x4E2, 0x4BF,
            0x4E3, 0x4E0, 0x4BD, 0x4D5, 0x4C3, 0x4C5, 0x4B5, 0x49D, 0x4E5,
            0x4D6, 0x4CA, 0x4B7, 0x4DE, 0x4DD, 0x4F3, 0x4F2, 0x4AB, 0x4ED,
            0x49F, 0x4B9, 0x4EC, 0x4D4, 0x4C0, 0x4BE, 0x4C4, 0x4C6, 0x4B6,
            0x49E, 0x4B8, 0x4AC, 0x4A0, 0x4BA, 0x4B2, 0x4A4, 0x4AE, 0x49C,
            0x4A6, 0x4C2, 0x4B4, 0x4A2, 0x4AA, 0x4BC, 0x4A8, 0x49A, 0x4B0,
            0x4E4, 0x4EE, 0x4CF, 0x4D2, 0x4CD, 0x4E6, 0x4E7, 0x4CE, 0x4DA,
            0x4E1, 0x4C8, 0x4B1, 0x4E8, 0x4E9, 0x4A3, 0x4EA, 0x4DF, 0x4AD,
            0x4D0, 0x4C7, 0x4F4, 0x49B, 0x4A5, 0x4C1, 0x4CC, 0x4D7, 0x4EF,
            0x4F0, 0x4D3, 0x75F, 0x4D9, 0x4B3, 0x4A1, 0x4DC, 0x4CB, 0x4A9,
            0x4BB, 0x4EB, 0x4A7, 0x4F1, 0x4C9, 0x499, 0x4DB, 0x4AF, 0x4D8,
            0x4D1, 0x1ED, 0x622, 0x623, 0x624, 0x625, 0x626, 0x528, 0x55D,
            0x584, 0x585, 0x586, 0x587, 0x588, 0x529, 0x5AA, 0x52A, 0x52B,
            0x52C, 0x559, 0x55A, 0x55B, 0x55C, 0x445, 0x464, 0x47B, 0x47C,
            0x47D, 0x47E, 0x47F, 0x446, 0x447, 0x448, 0x449, 0x460, 0x461,
            0x462, 0x463, 0x2F9, 0x2FA, 0x2FB, 0x2FC, 0x2FD, 0x27D, 0x27E,
            0x27F, 0x280, 0x281, 0x2BB, 0x2BC, 0x2BD, 0x2BE, 0x2BF, 0x677,
            0x678, 0x679, 0x67A, 0x67B, 0x5F1, 0x5F2, 0x5F3, 0x5F4, 0x5F5,
            0x5D5, 0x5D6, 0x5D7, 0x5D8, 0x5D9, 0x6DE, 0x6DF, 0x6E0, 0x6E1, 0x3F,
            0x40, 0x41, 0x42, 0x43, 0x12D, 0x12E, 0x12F, 0x130, 0x131, 0x1F8,
            0x1F9, 0x1FA, 0x1FB, 0x1FC, 0x223, 0x224, 0x225, 0x226, 0x227,
            0x152, 0x153, 0x154, 0x155, 0x156, 0x31B, 0x31C, 0x31D, 0x31E,
            0x31F, 0x4F5, 0x4F6, 0x4F7, 0x4F8, 0x4F9, 0xA7, 0xA8, 0xA9, 0xAA,
            0xAB, 0x17C, 0x17D, 0x17E, 0x17F, 0x180, 0x1D1, 0x1D2, 0x1D3, 0x1D4,
            0x1D5, 0x1A0, 0x1A1, 0x1A2, 0x1A3, 0x1A4, 0x772, 0x773, 0x774,
            0x775, 0x776, 0x494, 0x495, 0x496, 0x497, 0x498, 0x334, 0x3B3,
            0x42A, 0x42B, 0x42C, 0x42D, 0x42E, 0x335, 0x336, 0x337, 0x338,
            0x3AF, 0x3B0, 0x3B1, 0x3B2, 0x5CF, 0x68E, 0x68D, 0x29E, 0x725,
            0x6D4, 0x66D, 0x1F1, 0x122, 0x1BD, 0x273, 0x1F0, 0x15E, 0x16D, 0xCC,
            0x64, 0x565, 0x6F4, 0x176, 0x745, 0x596, 0x683, 0x69E, 0x76E, 0x6FD,
            0x6FF, 0x6CA, 0x685, 0x2A6, 0x2A3, 0x4FA, 0x4FB, 0x5AB, 0x34, 0x78F,
            0xA5, 0x76F, 0x179, 0x709, 0x3E, 0x279, 0x237, 0x1BE, 0x102, 0x98,
            0x70E, 0x647, 0x300, 0x5BA, 0x749, 0x797, 0x63F, 0x208, 0x103,
            0x714, 0x564, 0x203, 0x65E, 0x1C4, 0x188, 0x1C7, 0x1C6, 0x1D7,
            0x1D9, 0x14C, 0x6A8, 0x275, 0x202, 0x238, 0x261, 0x2DB, 0x2DC,
            0x576, 0x50F, 0x16B, 0x6A0, 0x21C, 0xE7, 0x7F, 0x6D6, 0x69F, 0x78B,
            0x693, 0x54B, 0x558, 0x527, 0x507, 0x550, 0x4FD, 0x552, 0x73C,
            0x3E9, 0x3F3, 0x413, 0x415, 0x3DD, 0x3DF, 0x3FB, 0x3F8, 0x405,
            0x40F, 0x780, 0x736, 0x3E3, 0x3ED, 0x5E7, 0x51C, 0x409, 0x5E2,
            0x77A, 0x114, 0x115, 0x324, 0x325, 0x63B, 0x5D2, 0x739, 0x6B, 0x36E,
            0x378, 0xD3, 0x127, 0x17, 0x69, 0xD1, 0x71, 0xD9, 0x2B, 0xC9, 0x61,
            0xB1, 0x49, 0x398, 0x39A, 0xCB, 0x63, 0x26, 0x3BD, 0x3C7, 0x54E,
            0xBF, 0x57, 0x41F, 0x3A4, 0x4B, 0xB3, 0x69A, 0x125, 0xCD, 0x65,
            0x362, 0x364, 0x1C, 0x4, 0x429, 0x3AE, 0x37D, 0x380, 0xC5, 0x5D,
            0x21, 0xD7, 0x6F, 0xB7, 0x4F, 0x59, 0x754, 0xC1, 0xAF, 0x47, 0xB9,
            0x51, 0x38A, 0x394, 0xD5, 0x6D, 0x7A3, 0x55, 0x67, 0x12, 0x126,
            0xC7, 0x5F, 0xB5, 0x4D, 0x3D1, 0x3DB, 0x356, 0x360, 0xBD, 0xCF,
            0xBB, 0x53, 0xAD, 0x45, 0xC, 0xC3, 0x5B, 0x342, 0x34C, 0x6A6, 0x6B3,
            0x46C, 0x471, 0x476, 0x467, 0x5D0, 0x2C6, 0x555, 0x5C5, 0xF7, 0x24A,
            0x26E, 0x313, 0x289, 0x480, 0x8F, 0x2B1, 0x67F, 0x2BA, 0x99, 0x192,
            0x56A, 0x11E, 0x11F, 0x32E, 0x395, 0x32F, 0x396, 0x410, 0x53F,
            0x6BD, 0x6BF, 0x515, 0x368, 0x372, 0x435, 0x430, 0x43A, 0x43F,
            0x2CF, 0x2D0, 0x3B7, 0x3C1, 0x419, 0x39E, 0x763, 0x504, 0x423,
            0x3A8, 0x794, 0x5B1, 0x593, 0x211, 0x751, 0x384, 0x38E, 0x3FF,
            0x79D, 0x5A9, 0x3CB, 0x3D5, 0x350, 0x35A, 0x53C, 0x5B5, 0x33C,
            0x346, 0x50E, 0x5DE, 0x55E, 0x189, 0x18B, 0x182, 0x183, 0x1CC,
            0x16F, 0x411, 0x785, 0x649, 0x1B, 0x6CB, 0x9B, 0x6D9, 0x782, 0x5D3,
            0x6A1, 0x6C2, 0x3C, 0x2E7, 0x2E8, 0x2C9, 0x2CA, 0x277, 0x2F3, 0x2F4,
            0x21A, 0x235, 0x1DF, 0x140, 0x161, 0x319, 0x29F, 0x68B, 0xE2, 0x7A,
            0x712, 0xE5, 0x7D, 0x1, 0x128, 0x2A7, 0x6D2, 0x2B6, 0x771, 0x634,
            0x666, 0x66C, 0x23C, 0xE0, 0x78, 0x68C, 0x769, 0x22D, 0x294, 0x62E,
            0x73D, 0x14B, 0x740, 0x247, 0x2E3, 0x2E4, 0x2C2, 0x2C3, 0x2DD,
            0x2DE, 0x2D1, 0x2D2, 0x2E9, 0x2EA, 0x2CB, 0x2CC, 0x10C, 0x10D,
            0x2D7, 0x2D8, 0x67D, 0x1CA, 0x66F, 0x1CE, 0x5AC, 0x633, 0x143, 0x3A,
            0x212, 0xF9, 0x30D, 0x286, 0x486, 0x91, 0x2AB, 0x267, 0x490, 0x508,
            0x5A5, 0x5CB, 0x676, 0x6D7, 0x538, 0x523, 0x65F, 0x67C, 0x653,
            0x6DD, 0x648, 0x650, 0x65B, 0x6BB, 0x658, 0x219, 0x292, 0x6F1,
            0x6EC, 0x53D, 0x36, 0xFA, 0x24C, 0x28C, 0x488, 0x92, 0x268, 0x245,
            0x492, 0x69D, 0x434, 0x42F, 0x439, 0x43E, 0xE1, 0x79, 0x536, 0x333,
            0x11A, 0x11B, 0x32A, 0x32B, 0x639, 0x6C5, 0x6C9, 0x6BC, 0x654,
            0x724, 0xED, 0x170, 0x85, 0x71E, 0x731, 0x6A2, 0x22C, 0x21D, 0x20B,
            0x63E, 0x258, 0x3B, 0x594, 0x72B, 0x5C6, 0x23F, 0xF4, 0x249, 0x26C,
            0x311, 0x288, 0x484, 0x8C, 0x2AF, 0x48E, 0x781, 0xDB, 0x73, 0x66E,
            0x19F, 0x660, 0x133, 0x18, 0x2C, 0x27, 0x1D, 0x5, 0x22, 0x13, 0xD,
            0x2ED, 0x2EE, 0x689, 0x68A, 0xC4, 0x207, 0x24B, 0x271, 0x28B, 0x5C,
            0x2B3, 0x19E, 0x700, 0x6C7, 0x10A, 0x10B, 0x13F, 0x15F, 0x317,
            0x1CB, 0x2A0, 0x20, 0x242, 0x13E, 0x16C, 0x644, 0x6EA, 0x701, 0x304,
            0x16E, 0x74C, 0x670, 0x17B, 0x144, 0x1E1, 0x720, 0x6CC, 0x27B,
            0x5ED, 0x239, 0x6AF, 0x70B, 0x75C, 0x742, 0x1B8, 0x65A, 0x6D5, 0xFB,
            0x24D, 0x28D, 0x489, 0x93, 0x246, 0x493, 0x278, 0x236, 0x1E4, 0x141,
            0x162, 0x21E, 0xA4, 0xD6, 0x6E, 0x20C, 0x722, 0x516, 0x500, 0x640,
            0x52E, 0x522, 0x659, 0x665, 0x221, 0x1AF, 0x691, 0x308, 0x190, 0x15,
            0x29, 0x759, 0x24, 0x757, 0x1A, 0x0, 0x1F, 0x10, 0x8, 0xFC, 0x94,
            0x199, 0x1E6, 0xB6, 0x1D8, 0x135, 0x4E, 0x158, 0x680, 0x3EB, 0x3E1,
            0x3FD, 0x407, 0x778, 0xFD, 0x366, 0x370, 0x3B5, 0x3BF, 0x417, 0x39C,
            0x766, 0x270, 0x57D, 0x315, 0x28A, 0x487, 0x421, 0x3A6, 0x232,
            0x382, 0x38C, 0x79B, 0x95, 0x2B2, 0x579, 0x3C9, 0x3D3, 0x34E, 0x358,
            0x491, 0x512, 0x503, 0x33A, 0x344, 0x6DC, 0x74B, 0x6, 0xE, 0x663,
            0x1F4, 0x1EE, 0x62A, 0x6B4, 0x621, 0x60C, 0x613, 0x61A, 0x5FC,
            0x603, 0x46E, 0x469, 0x473, 0x478, 0x252, 0x5A3, 0x5F0, 0x14A, 0xF2,
            0x50D, 0x30E, 0x482, 0x22F, 0x8A, 0x2AC, 0x269, 0x299, 0x48C, 0xA3,
            0x72D, 0x5A4, 0x216, 0x6E8, 0x747, 0x21F, 0x707, 0xF, 0x19, 0x2D,
            0x28, 0x1E, 0x7, 0x23, 0x14, 0x755, 0x164, 0x6A9, 0x6C6, 0x62C,
            0x1DE, 0x729, 0xC0, 0x243, 0x265, 0x30B, 0x284, 0x58, 0x2A9, 0x53A,
            0xE3, 0x7B, 0x546, 0x74E, 0x567, 0x599, 0x5BB, 0xDA, 0x72, 0x562,
            0x1B9, 0x19A, 0x165, 0x2E, 0x194, 0x74D, 0x554, 0x6F8, 0x151, 0x149,
            0x479, 0xA2, 0x109, 0x656, 0x129, 0x19D, 0x159, 0xAE, 0x1FE, 0x181,
            0x145, 0x46, 0x1B5, 0x3EF, 0x3E5, 0x40B, 0x77C, 0x25A, 0x25B, 0x1D6,
            0x63D, 0xB8, 0x1C2, 0x1DB, 0x262, 0x22E, 0x25C, 0x136, 0x50, 0x36A,
            0x374, 0x578, 0x3B9, 0x3C3, 0x41B, 0x3A0, 0x767, 0x50B, 0x425,
            0x3AA, 0x5BE, 0x386, 0x390, 0x401, 0x79F, 0x56D, 0x3CD, 0x3D7,
            0x352, 0x35C, 0x33E, 0x348, 0x4FE, 0x6C3, 0x193, 0x708, 0x11C,
            0x11D, 0x32C, 0x32D, 0x19C, 0xD4, 0x13B, 0x6C, 0x692, 0x1B7, 0x673,
            0x3E0, 0x3EA, 0x5E5, 0x406, 0x5DF, 0x777, 0x788, 0x365, 0x36F,
            0x540, 0x59D, 0x58D, 0x3B4, 0x3BE, 0x416, 0x39B, 0x5C0, 0x762,
            0x50A, 0x570, 0x667, 0x514, 0x672, 0x1CF, 0x420, 0x3A5, 0x75E,
            0x791, 0x5B0, 0x590, 0x752, 0x544, 0x381, 0x38B, 0x3FC, 0x79A, 0xDF,
            0x77, 0x3C8, 0x3D2, 0x34D, 0x357, 0x5B6, 0x339, 0x343, 0x6AB, 0x6B0,
            0x5DC, 0x6D1, 0x694, 0x39, 0x687, 0x2FE, 0x2FF, 0x6CE, 0x1E8, 0x27C,
            0x148, 0x82, 0xEA, 0x604, 0x1DC, 0x184, 0x18A, 0x72E, 0x6E7, 0x2C7,
            0x53B, 0x711, 0x6ED, 0x1C0, 0x5EC, 0x61F, 0x60A, 0x611, 0x618,
            0x5FA, 0x601, 0x12A, 0x73F, 0x595, 0x30, 0x6EE, 0x51B, 0x51A, 0x5D1,
            0x209, 0x64E, 0x305, 0x1C9, 0x748, 0x628, 0x196, 0x696, 0x73A,
            0x681, 0x637, 0x64F, 0x64A, 0x1F7, 0x6FC, 0x222, 0x1F2, 0x1E7,
            0x1F3, 0x6E4, 0x6D8, 0x104, 0x9D, 0x105, 0x9E, 0x758, 0x524, 0x241,
            0x1AE, 0x19B, 0x11, 0x3E8, 0x3F2, 0x414, 0x3DE, 0x3FA, 0x3F7, 0x40E,
            0x77F, 0x556, 0x123, 0x2F5, 0x2F6, 0x2EF, 0x2F0, 0x738, 0x36D,
            0x377, 0x397, 0x399, 0x412, 0x3BC, 0x3C6, 0x41E, 0x3A3, 0x361,
            0x363, 0x3DC, 0x3, 0x428, 0x3AD, 0x37C, 0x37F, 0x389, 0x393, 0x404,
            0x7A2, 0x3D0, 0x3DA, 0x355, 0x35F, 0xB, 0x341, 0x34B, 0x1AC, 0x1A6,
            0x684, 0x58E, 0x531, 0x6BA, 0x638, 0x629, 0x635, 0x636, 0x63C,
            0x63A, 0x730, 0x733, 0x627, 0x631, 0x630, 0x632, 0x509, 0x2B9,
            0x655, 0x543, 0x620, 0x60B, 0x612, 0x619, 0x5FB, 0x602, 0x768, 0xE6,
            0x7E, 0x274, 0x760, 0x671, 0x9C, 0x761, 0x172, 0x253, 0x255, 0x27A,
            0x25E, 0x2B7, 0x303, 0xEC, 0x147, 0x84, 0x20D, 0x21B, 0x166, 0x1BF,
            0x35, 0x307, 0x1EC, 0x18E, 0xC6, 0x139, 0x185, 0x5E, 0x15C, 0x65C,
            0x78D, 0x67E, 0x715, 0x2A4, 0x2A1, 0x1C5, 0x71A, 0xFE, 0x664, 0x1EF,
            0x68F, 0x661, 0x32, 0x1B6, 0x2C8, 0x197, 0x213, 0x734, 0x5AD, 0xB4,
            0x56B, 0x264, 0x30A, 0x4C, 0x2A8, 0x298, 0x57A, 0x1A9, 0x674, 0x17A,
            0x574, 0x5EB, 0x112, 0x113, 0x322, 0x323, 0x306, 0xEF, 0x87, 0x6AC,
            0x2A5, 0x716, 0x2A2, 0x71B, 0xFF, 0x14E, 0xDE, 0x76, 0x5EA, 0x59B,
            0x78E, 0x790, 0xA6, 0x6C0, 0xBC, 0x13C, 0x54, 0xCE, 0x210, 0x1E2,
            0x13A, 0x66, 0x641, 0x263, 0x146, 0x96, 0x18F, 0x62D, 0x6F3, 0x65D,
            0x2B8, 0x6AE, 0x198, 0x175, 0xBA, 0x282, 0x1DD, 0x137, 0x52, 0x15A,
            0x5AE, 0x741, 0x51E, 0x70A, 0x6AD, 0x76C, 0x52F, 0x651, 0x6E5, 0x9F,
            0x106, 0xDC, 0x74, 0x657, 0xAC, 0x132, 0x44, 0x157, 0x1A5, 0x1D0,
            0x783, 0x726, 0x214, 0x169, 0x452, 0x44D, 0x457, 0x45C, 0x451,
            0x44C, 0x456, 0x45B, 0x45E, 0x450, 0x44B, 0x455, 0x45A, 0x44F,
            0x44A, 0x454, 0x459, 0x453, 0x44E, 0x458, 0x45D, 0x5C4, 0x201,
            0x240, 0x1B2, 0x309, 0x187, 0x297, 0x6B9, 0x76B, 0x167, 0x501,
            0x61D, 0x608, 0x60F, 0x51F, 0x6AA, 0x616, 0x5F8, 0x535, 0x5FF,
            0x795, 0x796, 0x6F5, 0x200, 0xEE, 0x86, 0x9, 0xC2, 0x30C, 0x285,
            0x5A, 0x2AA, 0x266, 0x244, 0x78C, 0x784, 0x605, 0x75B, 0x6D3, 0x3E6,
            0x3F0, 0x5E9, 0x40C, 0x77D, 0xEB, 0x83, 0x6A7, 0x36B, 0x375, 0x5C8,
            0x438, 0x433, 0x43D, 0x442, 0x3BA, 0x3C4, 0x41C, 0x3A1, 0x589,
            0x5CA, 0x5EF, 0x58B, 0x575, 0x553, 0x786, 0x426, 0x3AB, 0x6BE,
            0x5B3, 0x591, 0x74F, 0x572, 0x387, 0x391, 0x402, 0x7A0, 0xE4, 0x7C,
            0x510, 0x3CE, 0x3D8, 0x353, 0x35D, 0x33F, 0x349, 0x6B1, 0x6DA,
            0x697, 0x69C, 0x521, 0x54D, 0x62B, 0x48A, 0x718, 0x583, 0x71D,
            0x721, 0x5CE, 0x59F, 0x110, 0x111, 0x4FF, 0x320, 0x321, 0x723,
            0x367, 0x371, 0x3B6, 0x3C0, 0x418, 0x39D, 0x548, 0x765, 0x520,
            0x542, 0x573, 0x422, 0x3A7, 0x793, 0x5BD, 0x592, 0x750, 0x53E,
            0x383, 0x38D, 0x3FE, 0x79C, 0x5A8, 0x3CA, 0x3D4, 0x34F, 0x359,
            0x56E, 0x33B, 0x345, 0x6DB, 0x5DD, 0x2D5, 0x2D6, 0x204, 0x291,
            0x234, 0x318, 0x3E2, 0x3EC, 0x5E8, 0x408, 0x5E0, 0x779, 0x2E5,
            0x2E6, 0x2C4, 0x2C5, 0x2DF, 0x2E0, 0x2D3, 0x2D4, 0x2EB, 0x2EC,
            0x2CD, 0x2CE, 0x10E, 0x10F, 0x2D9, 0x2DA };
        #endregion

        public ItemEditingForm()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
            DataLoaded = false;
            IsModified = false;
            Util.LoadItemNameList();

            // Initialize container object
            containers = new ItemContainer[] {
                new ItemContainer { Offset = 0x1A24, Count = MaxBagItem, Items = null, ContainerDataGridView = bagDataGridView, Tab = bagTab },
                new ItemContainer { Offset = 0x1ED4, Count = MaxContainerItem, Items = null, ContainerDataGridView = toolboxDataGridView, Tab = toolboxTab },
                new ItemContainer { Offset = 0x4DA8, Count = MaxContainerItem, Items = null, ContainerDataGridView = fridgeDataGridView, Tab = fridgeTab },
                new ItemContainer { Offset = 0x7C7C, Count = MaxContainerItem, Items = null, ContainerDataGridView = storageDataGridView, Tab = storageTab },
                new ItemContainer { Offset = 0xAB50, Count = MaxContainerItem, Items = null, ContainerDataGridView = materialDataGridView, Tab = materialTab },
                new ItemContainer { Offset = 0xDA24, Count = MaxContainerItem, Items = null, ContainerDataGridView = wardrobeDataGridView, Tab = wardrobeTab }
            };

            LoadItemData();
        }

        private void LoadItemData()
        {
            bagItemColumn.Items.Add("None");
            toolboxItemColumn.Items.Add("None");
            fridgeItemColumn.Items.Add("None");
            storageItemColumn.Items.Add("None");
            materialItemColumn.Items.Add("None");
            wardrobeItemColumn.Items.Add("None");
            for (int i = 0; i < Item.MaxItem; i++)
            {
                ushort itemIndex = ItemSortedList[i];
                bagItemColumn.Items.Add(Item.ItemNameList[itemIndex]);
                switch (Item.ItemContainer[itemIndex])
                {
                    case 0:
                        toolboxItemColumn.Items.Add(Item.ItemNameList[itemIndex]);
                        break;
                    case 1:
                        fridgeItemColumn.Items.Add(Item.ItemNameList[itemIndex]);
                        break;
                    case 2:
                        storageItemColumn.Items.Add(Item.ItemNameList[itemIndex]);
                        break;
                    case 3:
                        materialItemColumn.Items.Add(Item.ItemNameList[itemIndex]);
                        break;
                    case 4:
                        wardrobeItemColumn.Items.Add(Item.ItemNameList[itemIndex]);
                        break;
                }
            }

            bagStarColumn.Items.AddRange(StarQualityList);
            toolboxStarColumn.Items.AddRange(StarQualityList);
            fridgeStarColumn.Items.AddRange(StarQualityList);
            storageStarColumn.Items.AddRange(StarQualityList);
            materialStarColumn.Items.AddRange(StarQualityList);
            wardrobeStarColumn.Items.AddRange(StarQualityList);

            loadItem(ContainerID.Bag);
            loadItem(ContainerID.Toolbox);
            loadItem(ContainerID.Fridge);
            loadItem(ContainerID.Storage);
            loadItem(ContainerID.Material);
            loadItem(ContainerID.Wardrobe);

            bagDataGridView.RowCount = MaxBagItem;
            toolboxDataGridView.RowCount = MaxContainerItem;
            fridgeDataGridView.RowCount = MaxContainerItem;
            storageDataGridView.RowCount = MaxContainerItem;
            materialDataGridView.RowCount = MaxContainerItem;
            wardrobeDataGridView.RowCount = MaxContainerItem;
            
            DataLoaded = true;
        }

        // Load items from save data and sort them.
        private void loadItem(ContainerID containerID)
        {
            int count = containers[(int)containerID].Count;
            Item[] items = new Item[count];
            int localOffset = containers[(int)containerID].Offset;
            for (int i = 0; i < count; i++)
            {
                byte[] itemBytes = new byte[12];
                Array.Copy(MainForm.SaveData, localOffset, itemBytes, 0, 12);
                Item item = new Item(itemBytes);
                items[i] = item;
                if (item.Quality < -1)
                {
                    // Fix quality error in older version
                    int tempQuality = items[i].Quality & 0xFFFF;
                    if (tempQuality >= 0 && tempQuality <= Item.MaxQuality)
                        items[i].Quality = tempQuality;
                    else
                        items[i].Quality = Item.BaseQuality[items[i].Index];
                    IsModified = true;
                }
                localOffset += 12;
            }
            Array.Sort<Item>(items, (x, y) => x.CompareTo(y));
            containers[(int)containerID].Items = items;
        }

        #region CellValueNeeded
        // Display item data when needed
        private void dataGridView_CellValueNeeded(ContainerID containerID, DataGridViewCellValueEventArgs e)
        {
            Item[] items = containers[(int)containerID].Items;
            int col = e.ColumnIndex;
            int row = e.RowIndex;
            switch (col)
            {
                case 0:
                    e.Value = items[row].GetItemName();
                    break;
                case 1:
                    e.Value = 5;
                    if (items[row].Quality == -1)
                        e.Value = StarQualityList[0];
                    if (items[row].Quality == 0)
                        e.Value = StarQualityList[1];
                    if (items[row].Quality > 0 && items[row].Quality <= Item.MaxQuality)
                        e.Value =
                            StarQualityList[(items[row].Quality - 1) / 30 + 2];
                    if (items[row].Quality > Item.MaxQuality)
                    {
                        e.Value = StarQualityList[12];
                    }
                    break;
                case 2:
                    e.Value = items[row].Quality;
                    break;
                case 3:
                    e.Value = items[row].Quantity;
                    break;
            }
        }

        private void bagDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValueNeeded(ContainerID.Bag, e);
        }

        private void toolboxDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValueNeeded(ContainerID.Toolbox, e);
        }

        private void fridgeDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValueNeeded(ContainerID.Fridge, e);
        }

        private void storageDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValueNeeded(ContainerID.Storage, e);
        }

        private void materialDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValueNeeded(ContainerID.Material, e);
        }

        private void wardrobeDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValueNeeded(ContainerID.Wardrobe, e);
        }
        #endregion

        #region CellValuePushed
        // Push changes back to Item[] arrays
        private void dataGridView_CellValuePushed(ContainerID containerID, DataGridViewCellValueEventArgs e)
        {
            Item[] items = containers[(int)containerID].Items;
            DataGridView dataGridView = containers[(int)containerID].ContainerDataGridView;
            switch (e.ColumnIndex)
            {
                case 0:
                    int itemIndex = Array.IndexOf(Item.ItemNameList, e.Value);
                    if (itemIndex == -1)
                        items[e.RowIndex] = new Item();
                    else
                        items[e.RowIndex] = new Item((ushort)itemIndex);
                    dataGridView.InvalidateRow(e.RowIndex); // Repaint
                    break;
                case 1:
                    int starQualityIndex = Array.IndexOf(StarQualityList, e.Value.ToString());
                    if (starQualityIndex == 0)
                        items[e.RowIndex].Quality = -1;
                    else if (starQualityIndex == 12)
                        dataGridView.CancelEdit();
                    else
                        items[e.RowIndex].Quality = (starQualityIndex - 1) * 30;
                    dataGridView.InvalidateRow(e.RowIndex);
                    break;
                // Quality (column 2) and Quantity (column 3) were handled in CellValidating event.
            }
        }

        private void bagDataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValuePushed(ContainerID.Bag, e);
        }

        private void toolboxDataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValuePushed(ContainerID.Toolbox, e);
        }

        private void fridgeDataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValuePushed(ContainerID.Fridge, e);
        }

        private void storageDataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValuePushed(ContainerID.Storage, e);
        }

        private void materialDataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValuePushed(ContainerID.Material, e);
        }

        private void wardrobeDataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            dataGridView_CellValuePushed(ContainerID.Wardrobe, e);
        }
        #endregion

        #region CellFormatting
        // Grayed out some cells based on their value.
        private void dataGridView_CellFormatting(ContainerID containerID, DataGridViewCellFormattingEventArgs e)
        {
            Item[] items = containers[(int)containerID].Items;
            DataGridView dataGridView = containers[(int)containerID].ContainerDataGridView;
            switch (e.ColumnIndex)
            {
                case 1:
                case 2:
                    Item item = items[e.RowIndex];
                    if ((item.Index == 0xFFFF) ||
                        (item.Quality > Item.MaxQuality) ||
                        (Item.BaseQuality[item.Index] == -1))
                    {
                        e.CellStyle.BackColor = Color.LightGray;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true;
                    }
                    else
                    {
                        e.CellStyle.BackColor = dataGridView.DefaultCellStyle.BackColor;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;
                    }
                    break;
                case 3:
                    if ((items[e.RowIndex].Index == 0xFFFF) ||
                        (Item.BaseQuality[items[e.RowIndex].Index] == -1))
                    {
                        e.CellStyle.BackColor = Color.LightGray;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true;
                    }
                    else
                    {
                        e.CellStyle.BackColor = dataGridView.DefaultCellStyle.BackColor;
                        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;
                    }
                    break;
            }
        }

        private void bagDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView_CellFormatting(ContainerID.Bag, e);
        }

        private void toolboxDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView_CellFormatting(ContainerID.Toolbox, e);
        }

        private void fridgeDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView_CellFormatting(ContainerID.Fridge, e);
        }

        private void storageDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView_CellFormatting(ContainerID.Storage, e);
        }

        private void materialDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView_CellFormatting(ContainerID.Material, e);
        }

        private void wardrobeDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView_CellFormatting(ContainerID.Wardrobe, e);
        }
        #endregion

        #region CellClick
        // Show dropdown list right after clicking into ComboBox cells.
        private void dataGridView_CellClick(DataGridView dataGridView, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex != 0) && (e.ColumnIndex != 1)) return;
            ComboBox cb = (ComboBox) dataGridView.EditingControl;
            if (cb != null) cb.DroppedDown = true;
        }

        private void bagDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_CellClick(bagDataGridView, e);
        }

        private void toolboxDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_CellClick(toolboxDataGridView, e);
        }

        private void fridgeDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_CellClick(fridgeDataGridView, e);
        }

        private void storageDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_CellClick(storageDataGridView, e);
        }

        private void materialDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_CellClick(materialDataGridView, e);
        }

        private void wardrobeDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_CellClick(wardrobeDataGridView, e);
        }
        #endregion

        #region CurrentCellDirtyStateChanged
        // Commit cell value change for all ComboBox columns (column 0 and column 1).
        private void bagDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (bagDataGridView.IsCurrentCellDirty &&
                bagDataGridView.CurrentCell.ColumnIndex == 0)
            {
                bagDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void toolboxDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (toolboxDataGridView.IsCurrentCellDirty &&
                toolboxDataGridView.CurrentCell.ColumnIndex == 0)
            {
                toolboxDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void fridgeDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (fridgeDataGridView.IsCurrentCellDirty &&
                fridgeDataGridView.CurrentCell.ColumnIndex == 0)
            {
                fridgeDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void storageDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (storageDataGridView.IsCurrentCellDirty &&
                storageDataGridView.CurrentCell.ColumnIndex == 0)
            {
                storageDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void materialDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (materialDataGridView.IsCurrentCellDirty &&
                materialDataGridView.CurrentCell.ColumnIndex == 0)
            {
                materialDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void wardrobeDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (wardrobeDataGridView.IsCurrentCellDirty &&
                wardrobeDataGridView.CurrentCell.ColumnIndex == 0)
            {
                wardrobeDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        #endregion

        #region CellValidating
        // Validate and push new values into Item[] arrays.
        private void dataGridView_CellValidating(ContainerID containerID, DataGridViewCellValidatingEventArgs e)
        {
            DataGridView dataGridView = containers[(int)containerID].ContainerDataGridView;
            if (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly)
                return;
            switch (e.ColumnIndex)
            {
                case 2:
                    int quality;
                    bool isValidQuality = Int32.TryParse(e.FormattedValue.ToString(), out quality);
                    if (!isValidQuality || quality < -1 || quality > Item.MaxQuality)
                    {
                        dataGridView.Rows[e.RowIndex].Cells[2].ErrorText =
                            "Quality value must be a valid number between -1 and 300";
                        dataGridView.CancelEdit();
                    }
                    else
                    {
                        dataGridView.Rows[e.RowIndex].Cells[2].ErrorText = null;
                        containers[(int)containerID].Items[e.RowIndex].Quality = quality;
                    }
                    break;
                case 3:
                    byte quantity;
                    bool isValidQuantity = Byte.TryParse(e.FormattedValue.ToString(), out quantity);
                    if (!isValidQuantity || quantity < 1 || quantity > 99)
                    {
                        dataGridView.Rows[e.RowIndex].Cells[3].ErrorText =
                            "Must be a valid number between 1 and 99";
                        dataGridView.CancelEdit();
                    }
                    else
                    {
                        dataGridView.Rows[e.RowIndex].Cells[3].ErrorText = null;
                        containers[(int)containerID].Items[e.RowIndex].Quantity = quantity;
                    }
                    break;
            }
        }

        private void bagDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            dataGridView_CellValidating(ContainerID.Bag, e);
        }

        private void toolboxDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            dataGridView_CellValidating(ContainerID.Toolbox, e);
        }

        private void fridgeDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            dataGridView_CellValidating(ContainerID.Fridge, e);
        }

        private void storageDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            dataGridView_CellValidating(ContainerID.Storage, e);
        }

        private void materialDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            dataGridView_CellValidating(ContainerID.Material, e);
        }

        private void wardrobeDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            dataGridView_CellValidating(ContainerID.Wardrobe, e);
        }
        #endregion

        // Toggle between quality value and star quality.
        private void qualityCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (qualityCheckBox.Checked)
            {
                bagDataGridView.Columns[1].Visible = false;
                toolboxDataGridView.Columns[1].Visible = false;
                fridgeDataGridView.Columns[1].Visible = false;
                storageDataGridView.Columns[1].Visible = false;
                materialDataGridView.Columns[1].Visible = false;
                wardrobeDataGridView.Columns[1].Visible = false;
                bagDataGridView.Columns[2].Visible = true;
                toolboxDataGridView.Columns[2].Visible = true;
                fridgeDataGridView.Columns[2].Visible = true;
                storageDataGridView.Columns[2].Visible = true;
                materialDataGridView.Columns[2].Visible = true;
                wardrobeDataGridView.Columns[2].Visible = true;
            }
            else
            {
                bagDataGridView.Columns[1].Visible = true;
                toolboxDataGridView.Columns[1].Visible = true;
                fridgeDataGridView.Columns[1].Visible = true;
                storageDataGridView.Columns[1].Visible = true;
                materialDataGridView.Columns[1].Visible = true;
                wardrobeDataGridView.Columns[1].Visible = true;
                bagDataGridView.Columns[2].Visible = false;
                toolboxDataGridView.Columns[2].Visible = false;
                fridgeDataGridView.Columns[2].Visible = false;
                storageDataGridView.Columns[2].Visible = false;
                materialDataGridView.Columns[2].Visible = false;
                wardrobeDataGridView.Columns[2].Visible = false;
            }
        }

        // Save item data for specified container
        private void saveItems(ContainerID containerID)
        {
            Item[] items = containers[(int)containerID].Items;
            int count = containers[(int)containerID].Count;
            int offset = containers[(int)containerID].Offset;
            DataGridView dataGridView = containers[(int)containerID].ContainerDataGridView;
            byte[] itemBytes = new byte[12 * count];
            for (int i = 0; i < count; i++)
            {
                Array.Copy(items[i].ToArray(), 0, itemBytes, 12 * i, 12);
            }
            Array.Copy(itemBytes, 0, MainForm.SaveData, offset, 12 * count);
        }

        private void saveAllItems()
        {
            saveItems(ContainerID.Bag);
            saveItems(ContainerID.Toolbox);
            saveItems(ContainerID.Fridge);
            saveItems(ContainerID.Storage);
            saveItems(ContainerID.Material);
            saveItems(ContainerID.Wardrobe);
        }

        // Increase quantity of all items to 99.
        private void itemx99Button_Click(object sender, EventArgs e)
        {
            int containerID = itemTabControl.SelectedIndex;
            DataGridView dataGridView = containers[containerID].ContainerDataGridView;
            Item[] items = containers[containerID].Items;
            int count = containers[containerID].Count;

            if (dataGridView != null && items != null)
                for (int i = 0; i < count; i++)
                {
                    int itemIndex = items[i].Index;
                    if ((itemIndex == 0xFFFF) || (Item.BaseQuality[itemIndex] == -1)) continue;
                    items[i].Quantity = 99;
                    dataGridView.InvalidateRow(i);
                }
        }

        // Change quality of all items to maximum value if possible
        private void maxQualityButton_Click(object sender, EventArgs e)
        {
            int containerID = itemTabControl.SelectedIndex;
            DataGridView dataGridView = containers[containerID].ContainerDataGridView;
            Item[] items = containers[containerID].Items;
            int count = containers[containerID].Count;

            if (dataGridView != null && items != null)
                for (int i = 0; i < count; i++)
                {
                    //int itemIndex = Array.IndexOf(Item.ItemNameList, dataGridView.Rows[i].Cells[0].Value);
                    int itemIndex = items[i].Index;
                    if ((itemIndex == 0xFFFF) || (Item.BaseQuality[itemIndex] == -1)) continue;
                    items[i].Quality = Item.MaxQuality;
                    dataGridView.InvalidateRow(i);
                }
        }

        private void ItemEditingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveAllItems();
        }
    }
}
