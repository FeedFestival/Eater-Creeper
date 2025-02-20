using Game.Shared.Constants.EntityID;
using Game.Shared.Constants.Store;
using System;
using UnityEngine;

namespace Game.Shared.Constants {

    public enum EntityType {
        Unset = 0,
        Unit = 1,
        Interactable = 2,
        Zone = 3,
        General = 4,
        Item = 5,
        //
        Traversable = 6,    // Ladders, Cliffs

    }

    #region Interfaces
    public interface IEntityProps {
        EntityType type { get; set; }
    }
    public interface IUnitEntityProps : IEntityProps {
        UnitName name { get; set; }
    }
    public interface IInteractableEntityProps : IEntityProps {
        InteractableName name { get; set; }
    }
    public interface IZoneEntityProps : IEntityProps {
        Zone name { get; set; }
        SceneName sceneName { get; set; }
    }

    public interface IGeneralEntityProps : IEntityProps {
        EntityType entityType { get; set; }
        InteractType interactType { get; set; }
        int uniqueIndex { get; set; }
    }

    public interface IItemEntityProps : IEntityProps {
        ItemType itemType { get; set; }
        Item name { get; set; }
        ItemFor itemFor { get; set; }
    }

    //-------------------------

    public struct UnitEntityProps : IUnitEntityProps {
        public EntityType type { get; set; }
        public UnitName name { get; set; }
    }

    public struct InteractableEntityProps : IInteractableEntityProps {
        public EntityType type { get; set; }
        public InteractableName name { get; set; }
    }

    public struct ZoneEntityProps : IZoneEntityProps {
        public EntityType type { get; set; }
        public Zone name { get; set; }
        public SceneName sceneName { get; set; }
    }

    public struct GeneralEntityProps : IGeneralEntityProps {
        public EntityType type { get; set; }
        public EntityType entityType { get; set; }
        public InteractType interactType { get; set; }
        public int uniqueIndex { get; set; }
    }

    public struct ItemEntityProps : IItemEntityProps {
        public EntityType type { get; set; }
        public ItemType itemType { get; set; }
        public Item name { get; set; }
        public ItemFor itemFor { get; set; }
    }

    #endregion

    public static class EntityDefinition {

        public static string DecodeId(int entityId) {
            var props = DecodeIdProps(entityId);
            return DecodeId(props);
        }

        public static string DecodeId(IEntityProps props) {
            switch (props.type) {
                case EntityType.Unit:
                    var unitEntityProps = props as IUnitEntityProps;
                    return props.type + "." + unitEntityProps.name;
                case EntityType.Interactable:
                    var interactableProps = props as IInteractableEntityProps;
                    return props.type + "." + interactableProps.name;
                case EntityType.Zone:
                    var zoneProps = props as IZoneEntityProps;
                    return props.type + "." + zoneProps.name + "." + zoneProps.sceneName;
                case EntityType.General:
                    var generalProps = props as IGeneralEntityProps;
                    return props.type + "." + generalProps.entityType + "." + generalProps.interactType + "." + generalProps.uniqueIndex;
                case EntityType.Item:
                    var itemProps = props as IItemEntityProps;
                    return props.type + "." + itemProps.itemType + "." + itemProps.name + "." + itemProps.itemFor;
                case EntityType.Unset:
                default:
                    return null;
            }
        }

        public static T DecodeId<T>(int entityId) {
            var props = DecodeIdProps(entityId);
            return (T)props;
        }

        public static IEntityProps DecodeIdProps(int entityId) {
            var entityType = GetEntityTypeFromId(entityId);
            switch (entityType) {
                case EntityType.Unit:
                    var unitEntityProps = new UnitEntityProps() {
                        type = entityType,
                        name = DecodeUnit(entityId)
                    };
                    return unitEntityProps;
                case EntityType.Interactable:
                    var interactableEntityProps = new InteractableEntityProps() {
                        type = entityType,
                        name = DecodeInteractable(entityId)
                    };
                    return interactableEntityProps;
                case EntityType.Zone:
                    var tuple = DecodeZone(entityId);
                    return new ZoneEntityProps() {
                        type = entityType,
                        name = tuple.zone,
                        sceneName = tuple.sceneName
                    };
                case EntityType.General:
                    var decodedGeneral = DecodeGeneral(entityId);
                    return new GeneralEntityProps() {
                        type = entityType,
                        entityType = decodedGeneral.entityType,
                        interactType = decodedGeneral.interactType,
                        uniqueIndex = decodedGeneral.uniqueIndex
                    };
                case EntityType.Item:
                    var decodedItem = DecodeItem(entityId);
                    return new ItemEntityProps() {
                        type = entityType,
                        itemType = decodedItem.itemType,
                        name = decodedItem.item,
                        itemFor = decodedItem.itemFor
                    };
                case EntityType.Unset:
                default:
                    break;
            }
            return null;
        }

        public static EntityType GetEntityTypeFromId(int entityId) {
            if (entityId        < 999) {
                return EntityType.Unit;
            } else if (entityId >= 1000     && entityId < 9999) {
                return EntityType.Interactable;
            } else if (entityId >= 10000    && entityId < 99999) {
                return EntityType.Zone;
            } else if (entityId >= 100000   && entityId < 999999) {
                return EntityType.General;
            } else if (entityId >= 1000000  && entityId < 9999999) {
                return EntityType.Item;
            } else {
                return EntityType.Unset;
            }
        }

        //-------------------- UNIT

        public static int EncodeId(UnitName unitName) {
            int unitNr = (int)unitName;
            if (unitNr > 999) {
                Debug.LogError("Invalid UnitName");
            }
            return unitNr;
        }

        private static UnitName DecodeUnit(int entityId) {
            return Enum.IsDefined(typeof(UnitName), entityId) ? (UnitName)entityId : UnitName.None;
        }

        //-------------------- INTERACTABLE

        public static int EncodeId(InteractableName name) {
            // 1000 >= 9999
            int interactableNr = (int)name;
            if (interactableNr > 8999) {
                Debug.LogError("Invalid InteractableName");
            }
            return 1000 + interactableNr;
        }

        private static InteractableName DecodeInteractable(int entityId) {
            int interactNr = entityId - 1000;
            return Enum.IsDefined(typeof(InteractableName), interactNr)
                ? (InteractableName)interactNr
                : InteractableName.None;
        }

        //-------------------- ZONE

        public static int EncodeId(Zone zoneName, SceneName sceneName) {
            int zoneNr = (int)zoneName;
            if (zoneNr > 999) {
                Debug.LogError("Invalid ZoneName");
            }
            int sceneNr = (int)sceneName;
            if (sceneNr > 99) {
                Debug.LogError("Invalid Zone - SceneName");
            }
            zoneNr = 10000 + (zoneNr * 100) + sceneNr;

            return zoneNr;
        }

        private static (Zone zone, SceneName sceneName) DecodeZone(int entityId) {
            // 10101
            // 10101 % 100 = 1
            int sceneNr = entityId % 100;
            // 10101 - 1 = 10100
            // 10100 - 10000 = 100
            // 100 / 100 = 1
            int zoneNr = ((entityId - sceneNr) - 10000) / 100;

            var zone = Enum.IsDefined(typeof(Zone), zoneNr)
                ? (Zone)zoneNr
                : Zone.None;
            var sceneName = Enum.IsDefined(typeof(SceneName), sceneNr)
                ? (SceneName)sceneNr
                : SceneName.None;
            return (zone, sceneName);
        }

        //-------------------- GENERAL

        public static int EncodeId(EntityType entityType, InteractType interactType) {
            int entityTypeNr = (int)entityType * 100000;
            int interactTypeNr = (int)interactType * 10000;
            return entityTypeNr + interactTypeNr;
        }

        private static (EntityType entityType, InteractType interactType, int uniqueIndex) DecodeGeneral(int entityId) {
            // [1.0.0001]
            // 110 000 % 10000 = 1
            int uniqueIndex = entityId % 10000;
            // 110 000 % 100000 = 1
            int interactTypeNr = ((entityId - uniqueIndex) % 100000) / 10000;
            int entityTypeNr = entityId / 100000;

            var entityType = Enum.IsDefined(typeof(EntityType), entityTypeNr)
                ? (EntityType)entityTypeNr
                : EntityType.Unset;
            var interactType = Enum.IsDefined(typeof(InteractType), interactTypeNr)
                ? (InteractType)interactTypeNr
                : InteractType.Default;
            
            return (entityType, interactType, uniqueIndex);
        }

        //-------------------- ITEM

        public static int EncodeId(ItemType itemType, Item item, ItemFor itemFor) {
            // [1.000.000]

            int itemTypeNr = (int)itemType;
            if (itemTypeNr > 9) {
                Debug.LogError("Invalid Item ItemType");
            }
            itemTypeNr = itemTypeNr * 1000000;

            int itemNr = (int)item;
            if (itemNr > 999) {
                Debug.LogError("Invalid ItemName");
            }
            itemNr = itemNr * 1000;

            int itemForNr = (int)itemFor;
            if (itemForNr > 999) {
                Debug.LogError("Invalid ItemForNr");
            }

            return itemTypeNr + itemNr + itemForNr;
        }

        private static (ItemType itemType, Item item, ItemFor itemFor) DecodeItem(int entityId) {
            // [1.001.001]
            // 1 001 001 % 1000 = 
            int itemForNr = entityId % 1000;
            int aux = (entityId - itemForNr) / 1000;
            int itemNr = aux - 1000;
            int itemTypeNr = (aux - itemNr) / 1000;

            var itemType = Enum.IsDefined(typeof(ItemType), itemTypeNr)
                ? (ItemType)itemTypeNr
                : ItemType.Tool;
            var item = Enum.IsDefined(typeof(Item), itemNr)
                ? (Item)itemNr
                : Item.None;
            var itemFor = Enum.IsDefined(typeof(ItemFor), itemForNr)
                ? (ItemFor)itemForNr
                : ItemFor.None;
            return (itemType, item, itemFor);
        }
    }
}