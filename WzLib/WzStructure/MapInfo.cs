/*  MapleLib - A general-purpose MapleStory library
 * Copyright (C) 2009, 2010, 2015 Snow and haha01haha01
   
 * This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using MapleLib.WzLib.WzStructure.Data;
using MapleLib.WzLib.WzStructure;
using System.Drawing;
using MapleLib.Helpers;

namespace MapleLib.WzLib.WzStructure
{
    public class MapInfo //Credits to Bui for some of the info
    {
        public static MapInfo Default = new MapInfo();

        public MapInfo()
        {
        }

        public MapInfo(WzImage image, string strMapName, string strStreetName)
        {
            int? startHour;
            int? endHour;
            this.strMapName = strMapName;
            this.strStreetName = strStreetName;
            WzFile file = (WzFile)image.WzFileParent;
            string loggerSuffix = ", map " + image.Name + ((file != null) ? (" of version " + Enum.GetName(typeof(WzMapleVersion), file.MapleVersion) + ", v" + file.Version.ToString()) : "");
            foreach (WzImageProperty prop in image["info"].WzProperties) 
            {
                switch (prop.Name)
                {
                    case "bgm":
                        bgm = InfoTool.GetString(prop);
                        break;
                    case "cloud":
                        cloud = InfoTool.GetBool(prop);
                        break;
                    case "swim":
                        swim = InfoTool.GetBool(prop);
                        break;
                    case "forcedReturn":
                        forcedReturn = InfoTool.GetInt(prop);
                        break;
                    case "hideMinimap":
                        hideMinimap = InfoTool.GetBool(prop);
                        break;
                    case "mapDesc":
                        mapDesc = InfoTool.GetString(prop);
                        break;
                    case "mapName":
                        mapName = InfoTool.GetString(prop);
                        break;
                    case "mapMark":
                        mapMark = InfoTool.GetString(prop);
                        break;
                    case "mobRate":
                        mobRate = InfoTool.GetFloat(prop);
                        break;
                    case "moveLimit":
                        moveLimit = InfoTool.GetInt(prop);
                        break;
                    case "returnMap":
                        returnMap = InfoTool.GetInt(prop);
                        break;
                    case "town":
                        town = InfoTool.GetBool(prop);
                        break;
                    case "version":
                        version = InfoTool.GetInt(prop);
                        break;
                    case "fieldLimit":
                        int fl = InfoTool.GetInt(prop);
                        if (fl >= (int)Math.Pow(2, 23)) 
                        {
                            ErrorLogger.Log(ErrorLevel.IncorrectStructure, "Invalid fieldlimit " + fl.ToString() + loggerSuffix);
                            fl = fl & ((int)Math.Pow(2, 23) - 1);
                        }
                        fieldLimit = (FieldLimit)fl;
                        break;
                    case "VRTop":
                    case "VRBottom":
                    case "VRLeft":
                    case "VRRight":
                        break;
                    case "link":
                        //link = InfoTool.GetInt(prop);
                        break;
                    case "timeLimit":
                        timeLimit = InfoTool.GetInt(prop);
                        break;
                    case "lvLimit":
                        lvLimit = InfoTool.GetInt(prop);
                        break;
                    case "onFirstUserEnter":
                        onFirstUserEnter = InfoTool.GetString(prop);
                        break;
                    case "onUserEnter":
                        onUserEnter = InfoTool.GetString(prop);
                        break;
                    case "fly":
                        fly = InfoTool.GetBool(prop);
                        break;
                    case "noMapCmd":
                        noMapCmd = InfoTool.GetBool(prop);
                        break;
                    case "partyOnly":
                        partyOnly = InfoTool.GetBool(prop);
                        break;
                    case "fieldType":
                        int ft = InfoTool.GetInt(prop);
                        if (!Enum.IsDefined(typeof(FieldType), ft)) 
                        {
                            ErrorLogger.Log(ErrorLevel.IncorrectStructure, "Invalid fieldType " + ft.ToString() + loggerSuffix);
                            ft = 0;
                        }
                        fieldType = (FieldType)ft;
                        break;
                    case "miniMapOnOff":
                        miniMapOnOff = InfoTool.GetBool(prop);
                        break;
                    case "reactorShuffle":
                        reactorShuffle = InfoTool.GetBool(prop);
                        break;
                    case "reactorShuffleName":
                        reactorShuffleName = InfoTool.GetString(prop);
                        break;
                    case "personalShop":
                        personalShop = InfoTool.GetBool(prop);
                        break;
                    case "entrustedShop":
                        entrustedShop = InfoTool.GetBool(prop);
                        break;
                    case "effect":
                        effect = InfoTool.GetString(prop);
                        break;
                    case "lvForceMove":
                        lvForceMove = InfoTool.GetInt(prop);
                        break;
                    case "timeMob":
                        startHour = InfoTool.GetOptionalInt(prop["startHour"]);
                        endHour = InfoTool.GetOptionalInt(prop["endHour"]);
                        int? id = InfoTool.GetOptionalInt(prop["id"]);
                        string message = InfoTool.GetOptionalString(prop["message"]);
                        if (id == null || message == null || (startHour == null ^ endHour == null))
                        {
                            ErrorLogger.Log(ErrorLevel.IncorrectStructure, "timeMob" + loggerSuffix);
                        }
                        else
                            timeMob = new TimeMob((int?)startHour, (int?)endHour, (int)id, message);
                        break;
                    case "help":
                        help = InfoTool.GetString(prop);
                        break;
                    case "snow":
                        snow = InfoTool.GetBool(prop);
                        break;
                    case "rain":
                        rain = InfoTool.GetBool(prop);
                        break;
                    case "dropExpire":
                        dropExpire = InfoTool.GetInt(prop);
                        break;
                    case "decHP":
                        decHP = InfoTool.GetInt(prop);
                        break;
                    case "decInterval":
                        decInterval = InfoTool.GetInt(prop);
                        break;
                    case "autoLieDetector":
                        startHour = InfoTool.GetOptionalInt(prop["startHour"]);
                        endHour = InfoTool.GetOptionalInt(prop["endHour"]);
                        int? interval = InfoTool.GetOptionalInt(prop["interval"]);
                        int? propInt = InfoTool.GetOptionalInt(prop["prop"]);
                        if (startHour == null || endHour == null || interval == null || propInt == null)
                        {
                            ErrorLogger.Log(ErrorLevel.IncorrectStructure, "autoLieDetector" + loggerSuffix);
                        }
                        else
                            autoLieDetector = new AutoLieDetector((int)startHour, (int)endHour, (int)interval, (int)propInt);
                        break;
                    case "expeditionOnly":
                        expeditionOnly = InfoTool.GetBool(prop);
                        break;
                    case "fs":
                        fs = InfoTool.GetFloat(prop);
                        break;
                    case "protectItem":
                        protectItem = InfoTool.GetInt(prop);
                        break;
                    case "createMobInterval":
                        createMobInterval = InfoTool.GetInt(prop);
                        break;
                    case "fixedMobCapacity":
                        fixedMobCapacity = InfoTool.GetInt(prop);
                        break;
                    case "streetName":
                        streetName = InfoTool.GetString(prop);
                        break;
                    case "noRegenMap":
                        noRegenMap = InfoTool.GetBool(prop);
                        break;
                    case "allowedItems":
                        allowedItems = new List<int>();
                        if (prop.WzProperties != null && prop.WzProperties.Count > 0)
                            foreach (WzImageProperty item in prop.WzProperties)
                                allowedItems.Add(item.GetInt());
                        break;
                    case "recovery":
                        recovery = InfoTool.GetFloat(prop);
                        break;
                    case "blockPBossChange":
                        blockPBossChange = InfoTool.GetBool(prop);
                        break;
                    case "everlast":
                        everlast = InfoTool.GetBool(prop);
                        break;
                    case "damageCheckFree":
                        damageCheckFree = InfoTool.GetBool(prop);
                        break;
                    case "dropRate":
                        dropRate = InfoTool.GetFloat(prop);
                        break;
                    case "scrollDisable":
                        scrollDisable = InfoTool.GetBool(prop);
                        break;
                    case "needSkillForFly":
                        needSkillForFly = InfoTool.GetBool(prop);
                        break;
                    case "zakum2Hack":
                        zakum2Hack = InfoTool.GetBool(prop);
                        break;
                    case "allMoveCheck":
                        allMoveCheck = InfoTool.GetBool(prop);
                        break;
                    case "VRLimit":
                        VRLimit = InfoTool.GetBool(prop);
                        break;
                    case "consumeItemCoolTime":
                        consumeItemCoolTime = InfoTool.GetBool(prop);
                        break;
                    default:
                        ErrorLogger.Log(ErrorLevel.MissingFeature, "Unknown Prop: " + prop.Name + loggerSuffix);
                        additionalProps.Add(prop);
                        break;
                }
            }
            if (image["info"]["VRLeft"] != null)
            {
                WzImageProperty info = image["info"];
                int left = InfoTool.GetInt(info["VRLeft"]);
                int right = InfoTool.GetInt(info["VRRight"]);
                int top = InfoTool.GetInt(info["VRTop"]);
                int bottom = InfoTool.GetInt(info["VRBottom"]);
                VR = new Rectangle(left, top, right - left, bottom - top);
                officialVR = true;
            }
        }

        public void Save(WzImage dest)
        {
            WzSubProperty info = new WzSubProperty();
            info["bgm"] = InfoTool.SetString(bgm);
            info["cloud"] = InfoTool.SetBool(cloud);
            info["swim"] = InfoTool.SetBool(swim);
            info["forcedReturn"] = InfoTool.SetInt(forcedReturn);
            info["hideMinimap"] = InfoTool.SetBool(hideMinimap);
            info["mapDesc"] = InfoTool.SetOptionalString(mapDesc);
            info["mapName"] = InfoTool.SetOptionalString(mapDesc);
            info["mapMark"] = InfoTool.SetString(mapMark);
            info["mobRate"] = InfoTool.SetFloat(mobRate);
            info["moveLimit"] = InfoTool.SetOptionalInt(moveLimit);
            info["returnMap"] = InfoTool.SetInt(returnMap);
            info["town"] = InfoTool.SetBool(town);
            info["version"] = InfoTool.SetInt(version);
            info["fieldLimit"] = InfoTool.SetInt((int)fieldLimit);
            info["timeLimit"] = InfoTool.SetOptionalInt(timeLimit);
            info["lvLimit"] = InfoTool.SetOptionalInt(lvLimit);
            info["onFirstUserEnter"] = InfoTool.SetOptionalString(onFirstUserEnter);
            info["onUserEnter"] = InfoTool.SetOptionalString(onUserEnter);
            info["fly"] = InfoTool.SetBool(fly);
            info["noMapCmd"] = InfoTool.SetBool(noMapCmd);
            info["partyOnly"] = InfoTool.SetBool(partyOnly);
            info["fieldType"] = InfoTool.SetOptionalInt((int?)fieldType);
            info["miniMapOnOff"] = InfoTool.SetBool(miniMapOnOff);
            info["reactorShuffle"] = InfoTool.SetBool(reactorShuffle);
            info["reactorShuffleName"] = InfoTool.SetOptionalString(reactorShuffleName);
            info["personalShop"] = InfoTool.SetBool(personalShop);
            info["entrustedShop"] = InfoTool.SetBool(entrustedShop);
            info["effect"] = InfoTool.SetOptionalString(effect);
            info["lvForceMove"] = InfoTool.SetOptionalInt(lvForceMove);
            if (timeMob != null) 
            {
                WzSubProperty prop = new WzSubProperty();
                prop["startHour"] = InfoTool.SetOptionalInt(timeMob.Value.startHour);
                prop["endHour"] = InfoTool.SetOptionalInt(timeMob.Value.endHour);
                prop["id"] = InfoTool.SetOptionalInt(timeMob.Value.id);
                prop["message"] = InfoTool.SetOptionalString(timeMob.Value.message);
                info["timeMob"] = prop;
            }
            info["help"] = InfoTool.SetOptionalString(help);
            info["snow"] = InfoTool.SetBool(snow);
            info["rain"] = InfoTool.SetBool(rain);
            info["dropExpire"] = InfoTool.SetOptionalInt(dropExpire);
            info["decHP"] = InfoTool.SetOptionalInt(decHP);
            info["decInterval"] = InfoTool.SetOptionalInt(decInterval);
            if (autoLieDetector != null) 
            {
                WzSubProperty prop = new WzSubProperty();
                prop["startHour"] = InfoTool.SetOptionalInt(autoLieDetector.Value.startHour);
                prop["endHour"] = InfoTool.SetOptionalInt(autoLieDetector.Value.endHour);
                prop["interval"] = InfoTool.SetOptionalInt(autoLieDetector.Value.interval);
                prop["prop"] = InfoTool.SetOptionalInt(autoLieDetector.Value.prop);
                info["autoLieDetector"] = prop;
            }
            info["expeditionOnly"] = InfoTool.SetBool(expeditionOnly);
            info["fs"] = InfoTool.SetOptionalFloat(fs);
            info["protectItem"] = InfoTool.SetOptionalInt(protectItem);
            info["createMobInterval"] = InfoTool.SetOptionalInt(createMobInterval);
            info["fixedMobCapacity"] = InfoTool.SetOptionalInt(fixedMobCapacity);
            info["streetName"] = InfoTool.SetOptionalString(streetName);
            info["noRegenMap"] = InfoTool.SetBool(noRegenMap);
            if (allowedItems != null)
            {
                WzSubProperty prop = new WzSubProperty();
                ErrorLogger.Log(ErrorLevel.MissingFeature, "I don't know how to repack allowedItems");
                info["allowedItems"] = prop;
            }
            info["recovery"] = InfoTool.SetOptionalFloat(recovery);
            info["blockPBossChange"] = InfoTool.SetBool(blockPBossChange);
            info["everlast"] = InfoTool.SetBool(everlast);
            info["damageCheckFree"] = InfoTool.SetBool(damageCheckFree);
            info["dropRate"] = InfoTool.SetOptionalFloat(dropRate);
            info["scrollDisable"] = InfoTool.SetBool(scrollDisable);
            info["needSkillForFly"] = InfoTool.SetBool(needSkillForFly);
            info["zakum2Hack"] = InfoTool.SetBool(zakum2Hack);
            info["allMoveCheck"] = InfoTool.SetBool(allMoveCheck);
            info["VRLimit"] = InfoTool.SetBool(VRLimit);
            info["consumeItemCoolTime"] = InfoTool.SetBool(consumeItemCoolTime);
            foreach (WzImageProperty prop in additionalProps) 
            {
                info.AddProperty(prop);
            }
            if (officialVR)
            {
                info["VRLeft"] = InfoTool.SetInt(VR.Value.Left);
                info["VRRight"] = InfoTool.SetInt(VR.Value.Right);
                info["VRTop"] = InfoTool.SetInt(VR.Value.Top);
                info["VRBottom"] = InfoTool.SetInt(VR.Value.Bottom);
            }
            dest["info"] = info;
        }

        //Cannot change
        public int version = 10;

        //Must have
        public string bgm = "Bgm00/GoPicnic";
        public string mapMark = "None";
        public FieldLimit fieldLimit = FieldLimit.FIELDOPT_NONE;
        public int returnMap = 999999999;
        public int forcedReturn = 999999999;
        public bool cloud = false;
        public bool swim = false;
        public bool hideMinimap = false;
        public bool town = false;
        public float mobRate = 1.5f;

        //Optional
        //public int link = -1;
        public int? timeLimit = null;
        public int? lvLimit = null;
        public FieldType? fieldType = null;
        public string onFirstUserEnter = null;
        public string onUserEnter = null;
        public MapleBool fly = null;
        public MapleBool noMapCmd = null;
        public MapleBool partyOnly = null;
        public MapleBool reactorShuffle = null;
        public string reactorShuffleName = null;
        public MapleBool personalShop = null;
        public MapleBool entrustedShop = null;
        public string effect = null; //Bubbling; 610030550 and many others
        public int? lvForceMove = null; //limit FROM value
        public TimeMob? timeMob = null;
        public string help = null; //help string
        public MapleBool snow = null;
        public MapleBool rain = null;
        public int? dropExpire = null; //in seconds
        public int? decHP = null;
        public int? decInterval = null;
        public AutoLieDetector? autoLieDetector = null;
        public MapleBool expeditionOnly = null;
        public float? fs = null; //slip on ice speed, default 0.2
        public int? protectItem = null; //ID, item protecting from cold
        public int? createMobInterval = null; //used for massacre pqs
        public int? fixedMobCapacity = null; //mob capacity to target (used for massacre pqs)

        //Unknown optional
        public int? moveLimit = null;
        public string mapDesc = null;
        public string mapName = null;
        public string streetName = null;
        public MapleBool miniMapOnOff = null;
        public MapleBool noRegenMap = null; //610030400
        public List<int> allowedItems = null;
        public float? recovery = null; //recovery rate, like in sauna (3)
        public MapleBool blockPBossChange = null; //something with monster carnival
        public MapleBool everlast = null; //something with bonus stages of PQs
        public MapleBool damageCheckFree = null; //something with fishing event
        public float? dropRate = null;
        public MapleBool scrollDisable = null;
        public MapleBool needSkillForFly = null;
        public MapleBool zakum2Hack = null; //JQ hack protection
        public MapleBool allMoveCheck = null; //another JQ hack protection
        public MapleBool VRLimit = null; //use vr's as limits?
        public MapleBool consumeItemCoolTime = null; //cool time of consume item
        public Coconut? coconut = null; // Some stupid ass event, see documentation of the struct
        public Snowball? snowBall = null; // ditto
        public MonsterCarnival? monsterCarnival = null;

        //Special
        public bool officialVR = false;
        public Rectangle? VR = null;
        public List<WzImageProperty> additionalProps = new List<WzImageProperty>();
        public string strMapName = "<Untitled>";
        public string strStreetName = "<Untitled>";
        public int id = 0;

        //Editor related, not actual properties
        public MapType mapType = MapType.RegularMap;

        public struct TimeMob
        {
            public int? startHour, endHour;
            public int id;
            public string message;

            public TimeMob(int? startHour, int? endHour, int id, string message)
            {
                this.startHour = startHour;
                this.endHour = endHour;
                this.id = id;
                this.message = message;
            }
        }

        public struct AutoLieDetector
        {
            public int startHour, endHour, interval, prop; //interval in mins, prop default = 80

            public AutoLieDetector(int startHour, int endHour, int interval, int prop)
            {
                this.startHour = startHour;
                this.endHour = endHour;
                this.interval = interval;
                this.prop = prop;
            }
        }

        // This is the dumbest of the features I have seen in the whole WZ structure. I do not believe that I'm actually
        // implementing a struct of this size for an event that probably never happened outside of beta phase.
        public struct Coconut
        {
            public int avatar00cap, avatar01cap, avatar10cap, avatar11cap, avatar00clothes, avatar01clothes, avatar10clothes, avatar11clothes, 
                countBombing, countFalling, countHit, countStopped, 
                timeDefault, timeFinished, timeExpand, timeMessage;
            public string effectLose, effectWin, eventName, eventObjectName, soundLose, soundWin;

            public Coconut(int avatar00cap, int avatar01cap, int avatar10cap, int avatar11cap, int avatar00clothes, int avatar01clothes, int avatar10clothes, int avatar11clothes, 
                int countBombing, int countFalling, int countHit, int countStopped, int timeDefault, int timeFinished, int timeExpand, int timeMessage,
                string effectLose, string effectWin, string eventName, string eventObjectName, string soundLose, string soundWin)
            {
                this.avatar00cap = avatar00cap;
                this.avatar01cap = avatar01cap;
                this.avatar10cap = avatar10cap;
                this.avatar11cap = avatar11cap;
                this.avatar00clothes = avatar00clothes;
                this.avatar01clothes = avatar01clothes;
                this.avatar10clothes = avatar10clothes;
                this.avatar11clothes = avatar11clothes;
                this.countBombing = countBombing;
                this.countFalling = countFalling;
                this.countHit = countHit;
                this.countStopped = countStopped;
                this.timeDefault = timeDefault;
                this.timeFinished = timeFinished;
                this.timeExpand = timeExpand;
                this.timeMessage = timeMessage;
                this.effectLose = effectLose;
                this.effectWin = effectWin;
                this.eventName = eventName;
                this.eventObjectName = eventObjectName;
                this.soundLose = soundLose;
                this.soundWin = soundWin;
            }
        }

        // Another event that never saw the light of day, except for once or twice in beta
        public struct Snowball
        {
            string portal0;
            string snowBall0;
            string snowMan0;
            int y0;
            string portal1;
            string snowBall1;
            string snowMan1;
            int y1;
            string damageSnowBall;
            int damageSnowMan0;
            int damageSnowMan1;
            int dx, recoveryAmount, section1, section2, section3, snowManHP, snowManWait, speed, x, x0, xMax, xMin;

            public Snowball(string portal0, string snowBall0, string snowMan0, int y0, string portal1, string snowBall1, string snowMan1, int y1, string damageSnowBall, int damageSnowMan0,
            int damageSnowMan1, int dx, int recoveryAmount, int section1, int section2, int section3, int snowManHP, int snowManWait, int speed, int x, int x0, int xMax, int xMin)
            {
                this.portal0 = portal0;
                this.snowBall0 = snowBall0;
                this.snowMan0 = snowMan0;
                this.y0 = y0;
                this.portal1 = portal1;
                this.snowBall1 = snowBall1;
                this.snowMan1 = snowMan1;
                this.y1 = y1;
                this.damageSnowBall = damageSnowBall;
                this.damageSnowMan0 = damageSnowMan0;
                this.damageSnowMan1 = damageSnowMan1;
                this.dx = dx;
                this.recoveryAmount = recoveryAmount;
                this.section1 = section1;
                this.section2 = section2;
                this.section3 = section3;
                this.snowManHP = snowManHP;
                this.snowManWait = snowManWait;
                this.speed = speed;
                this.x = x;
                this.x0 = x0;
                this.xMax = xMax;
                this.xMin = xMin;
            }
        }

        public struct MonsterCarnival
        {
            public int deathCP;
            public string effectLose, effectWin;
            public int[] guardian;
            public int[] mobID;
            public int[] mobTime;
            public int[] spendCP;
            //guardianGenPos and mobGenPos are in the editor
            public int mapDivided, reactorBlue, reactorRed;
            public float rewardClimax;
            public int[] reward_cpDiff;
            public float[] probChange_loseCoin;
            public float[] probChange_loseCP;
            public float[] probChange_loseNuff;
            public float[] probChange_loseRecovery;
            public float[] probChange_wInCoin;
            public float[] probChange_winCP;
            public float[] probChange_winNuff;
            public float[] probChange_winRecovery;
            public int rewardMapLose, rewardMapWin;
            public int[] skill;
            public string soundLose, soundWin;
            public int timeDefault, timeExpand, timeFinish, timeMessage;
        }
    }
}