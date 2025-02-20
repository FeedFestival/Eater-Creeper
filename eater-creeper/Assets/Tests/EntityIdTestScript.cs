using System.Collections;
using System.Collections.Generic;
using Game.Shared.Constants;
using Game.Shared.Constants.EntityID;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EntityIdTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void EntityIdTestScriptSimplePasses()
    {
        //-------------------- UNIT
        var id = EntityDefinition.EncodeId(UnitName.MainCharacter);

        var unitEntityProps = EntityDefinition.DecodeId<IUnitEntityProps>(id);

        Assert.AreEqual(unitEntityProps.type, EntityType.Unit);
        Assert.AreEqual(unitEntityProps.name, UnitName.MainCharacter);

        var unitName = EntityDefinition.DecodeId(unitEntityProps);

        Assert.AreEqual(unitName, "Unit.MainCharacter");

        //-------------------- INTERACT
        id = EntityDefinition.EncodeId(InteractableName.Gate);

        var interactableEntityProps = EntityDefinition.DecodeId<IInteractableEntityProps>(id);

        Assert.AreEqual(interactableEntityProps.type, EntityType.Interactable);
        Assert.AreEqual(interactableEntityProps.name, InteractableName.Gate);

        var interactableName = EntityDefinition.DecodeId(interactableEntityProps);

        Assert.AreEqual(interactableName, "Interactable.Gate");

        //-------------------- ZONE
        id = EntityDefinition.EncodeId(Zone.CutsceneStart, SceneName.Scene1);

        var zoneEntityProps = EntityDefinition.DecodeId<IZoneEntityProps>(id);

        Assert.AreEqual(zoneEntityProps.type, EntityType.Zone);
        Assert.AreEqual(zoneEntityProps.name, Zone.CutsceneStart);
        Assert.AreEqual(zoneEntityProps.sceneName, SceneName.Scene1);

        var zoneName = EntityDefinition.DecodeId(zoneEntityProps);

        Assert.AreEqual(zoneName, "Zone.CutsceneStart.Scene1");

        //-------------------- GENERAL
        id = EntityDefinition.EncodeId(EntityType.Traversable, InteractType.Traverse);

        var generalEntityProps = EntityDefinition.DecodeId<IGeneralEntityProps>(id);

        Assert.AreEqual(generalEntityProps.type, EntityType.General);
        Assert.AreEqual(generalEntityProps.entityType, EntityType.Traversable);
        Assert.AreEqual(generalEntityProps.interactType, InteractType.Traverse);
        Assert.AreEqual(generalEntityProps.uniqueIndex, 0);

        var generalName = EntityDefinition.DecodeId(generalEntityProps);

        Assert.AreEqual(generalName, "General.Traversable.Traverse.0");

        //-------------------- ITEM
        id = EntityDefinition.EncodeId(ItemType.Tool, Item.Key, ItemFor.Door_In_Scene1);
        
        var itemEntityProps = EntityDefinition.DecodeId<IItemEntityProps>(id);

        Assert.AreEqual(itemEntityProps.type, EntityType.Item);
        Assert.AreEqual(itemEntityProps.itemType, ItemType.Tool);
        Assert.AreEqual(itemEntityProps.name, Item.Key);
        Assert.AreEqual(itemEntityProps.itemFor, ItemFor.Door_In_Scene1);

        var itemName = EntityDefinition.DecodeId(itemEntityProps);

        Assert.AreEqual(itemName, "Item.Tool.Key.Door_In_Scene1");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator EntityIdTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
