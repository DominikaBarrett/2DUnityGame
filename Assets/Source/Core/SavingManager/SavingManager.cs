using System.Collections.Generic;
using System.IO;
using DungeonCrawl.Actors.Characters;
using DungeonCrawl.Actors.Items;
using Source.Actors.Characters;
using UnityEngine;
using Source.Actors.Items;

namespace Source.Core.SavingManager
{
    public class SavingManager : MonoBehaviour
    {
        private string _fileName = "SaveData.json";
        private string _savingPath;
        public static SavingManager Singleton { get; private set; }

        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }

            Singleton = this;
            _savingPath = Application.dataPath + "\\save";
            if (!Directory.Exists(_savingPath))
            {
                Directory.CreateDirectory(_savingPath);
            }
        }

        private void Start()
        {
            // TODO Remove this, it's for testing
            var json = JsonUtility.ToJson(GenerateSave());
            Debug.Log(json);
            WriteSaveToFile(GenerateSave());
            Save save = LoadFromFileSave();
            Debug.Log(save.player.position);
        }

        public void SaveGame()
        {
            WriteSaveToFile(GenerateSave());
        }

        /// <summary>
        /// Read game state and return save object.
        /// </summary>
        /// <returns>State of the game.</returns>
        private Save GenerateSave()
        {
            Save save = new Save();
            // Save player data
            save.player = GeneratePlayerSaveData();

            // Save characters states
            save.characters = GenerateCharactersSaveData();

            // Save items states
            save.items = GenerateItemsSaveData();

            return save;
        }

        private List<CharactersSaveData> GenerateCharactersSaveData()
        {
            // Get items in scene and fill list by CharacterSaveData objects
            List<CharactersSaveData> charactersList = new List<CharactersSaveData>();

            var charactersObjects = GameObject.FindGameObjectsWithTag("Character");
            foreach (var characterObject in charactersObjects)
            {
                Character character = characterObject.GetComponent<Character>();
                CharactersSaveData characterSaveData = new CharactersSaveData(character);
                charactersList.Add(characterSaveData);
            }

            return charactersList;
        }

        private List<ItemsSaveData> GenerateItemsSaveData()
        {
            // Get items in scene and fill list by ItemSaveData objects

            var itemsObjects = GameObject.FindGameObjectsWithTag("Item");
            List<ItemsSaveData> itemsList = new List<ItemsSaveData>();

            foreach (var itemObject in itemsObjects)
            {
                Item item = itemObject.GetComponent<Item>();
                ItemsSaveData itemsSaveData = new ItemsSaveData(item);
                itemsList.Add(itemsSaveData);
            }

            return itemsList;
        }

        private PlayerSaveData GeneratePlayerSaveData()
        {
            var playerGameObject = GameObject.FindGameObjectWithTag("Player");
            Player player = playerGameObject.GetComponent<Player>();
            PlayerSaveData playerData = new PlayerSaveData(player);
            return playerData;
        }

        private Save LoadFromFileSave()
        {
            string path = Path.Combine(_savingPath, _fileName);

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Save save = JsonUtility.FromJson<Save>(json);
                return save;
            }

            Debug.LogError("Could not Open the file " + _fileName + "to read");
            return null;
        }

        private void WriteSaveToFile(Save save)
        {
            var json = JsonUtility.ToJson(GenerateSave());
            string path = Path.Combine(_savingPath, _fileName);

            File.WriteAllText(path, json);
        }

        public void LoadSave(Save save)
        {
            // Player
            var playerGameObject = GameObject.FindGameObjectWithTag("Player");
            var player = playerGameObject.GetComponent<Player>();
            player.Position = save.player.position;
            player.CurrentHealth = save.player.currentHealth;
            player.MaxHealth = save.player.maxHealth;
            //Player Inventory
            var dictOfItems = GenerateDictOfItemById();
            var playerInventoryGameObject = GameObject.FindGameObjectWithTag("Inventory");
            var playerInventory = playerInventoryGameObject.GetComponent<Inventory>();

            foreach (var id in save.player.inventory)
            {
                playerInventory.AddItem(dictOfItems[id]);
            }
            
            //Character
            var dictOfCharacter = GenerateDictOfCharacterById();
            foreach (var saveCharacter in save.characters)
            {
                var character = dictOfCharacter[saveCharacter.id];
                character.gameObject.SetActive(saveCharacter.enabled);
                character.Position = saveCharacter.position;
                character.CurrentHealth = saveCharacter.currentHealth;
            }

            //Items
            foreach (var itemsSaveData  in save.items)
            {
                var item = dictOfItems[itemsSaveData.id];
                item.Amount = itemsSaveData.amount;
                item.enabled = itemsSaveData.enabled;
            }
            
        }

        public Dictionary<string, Item> GenerateDictOfItemById()
        {
            var itemGameObject = GameObject.FindGameObjectsWithTag("Item");
            Dictionary<string, Item> dict = new Dictionary<string, Item>();
            foreach (var item in itemGameObject)
            {
                var component = item.GetComponent<Item>();
                dict.Add(component.Id, component);
            }

            return dict;
        }

        public  Dictionary<string, Character> GenerateDictOfCharacterById()
        {
            var characterGameObject = GameObject.FindGameObjectsWithTag("Character");
            Dictionary<string, Character> dict = new Dictionary<string, Character>();
            foreach (var character in characterGameObject)
            {
                var component = character.GetComponent<Character>();
                dict.Add(component.Id,component);
            }

            return dict;
        }

   
    }
}