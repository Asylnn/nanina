<script lang="ts">
import type Equipment from '@/classes/item/equipment';
import InventoryPage from '../../Page/InventoryPage.vue';
import User from '@/classes/user/user';
import Item from '@/classes/item/item';
import ItemType from '@/classes/item/item_type';
import ItemManagerComponent from './ItemManagerComponent.vue';
import ClientResponseType from '@/classes/client_response_type';
import type { PropType } from 'vue';
import type Dictionary from '@/classes/dictionary';

/*Selectionner un user pour regarder son inventaire et pouvoir le modifier ##PLUS TARD
* Ajouter un item -> 
* Modifier un item (objet perso)
* Supprimer un item
*
* Add -> Confirmation puis ajout l'item selectionné en changeant la valeur count en la valeur mise dans le label
* Bouton update -> truc en dessous avec toutes ses infos et apres un autre bouton qui update les nouvelles infos (y compris la quantité)
* Delete -> Confirmation puis delete une quantité de fois l'item selectionné
* 
*/



export default {
    name : "InventoryManagerPage",
    data() {
        return {
            filteredType : ItemType.Equipment,
            modeManager : "add",
            ItemType : ItemType,
            publicPath : import.meta.env.BASE_URL,
        }
    },
    props : {
        user: {
            type: User,
            required: true
        },
        items: { //type dans l'ordre du select
            type: Object as PropType<Dictionary<Item>>,
            required: true
        },
    },
    components: {
        InventoryPage,
        ItemManagerComponent,
    },
    methods: {
        changeCateg(type : ItemType){
            this.filteredType = type
        },
        changeMode(mode : string) {
            this.modeManager = mode
        },

        deleteEquipement(id: number){
            delete this.user.inventory.equipment[id]
        },
        deleteItem(id: number){
            delete this.user.inventory.items[id]
        },
        addItem(item: Item)
        {
            switch(true){
                case this.filteredType == ItemType.Equipment:
                    this.user.inventory.AddEquipment(item as Equipment)
                    break;
                case this.filteredType == ItemType.WaifuConsumable || this.filteredType == ItemType.Material || this.filteredType == ItemType.UserConsumable:
                    console.log("add item")
                    this.user.inventory.AddItem(item)
                    break;
            }
        },

        UpdateDatabase(){
            this.SendToServer(ClientResponseType.UpdateUserInventory, JSON.stringify(this.user.inventory), this.user.Id)
        }
    },
}


</script>
<template>
    <div id="inventoryManager">
        <div id="theInventory">
            <div v-for="item in [...Object.values(user.inventory.equipment), ...Object.values(user.inventory.items)]">
                <div v-if="item.type == ItemType.Equipment">
                    <ItemManagerComponent :item="item" @delete-item="deleteEquipement" :forInventoryManager="true"></ItemManagerComponent>
                </div>
                <div v-else-if="item.type == ItemType.UserConsumable">
                    <ItemManagerComponent :item="item" @delete-item="deleteItem" :forInventoryManager="true"></ItemManagerComponent>
                </div>
                <div v-else-if="item.type == ItemType.WaifuConsumable">
                    <ItemManagerComponent :item="item" @delete-item="deleteItem" :forInventoryManager="true"></ItemManagerComponent>
                </div>
                <div v-else-if="item.type == ItemType.Material">
                    <ItemManagerComponent :item="item" @delete-item="deleteItem" :forInventoryManager="true"></ItemManagerComponent>
                </div>
                <!--You need to have the last case with a condition since one of the properties of user.inventory is a number and "for x in 245" break some stuff-->
            </div>
        </div>
        <div id="theManager">
            <div id="filterCategManager">
                <label>Type : </label>
                <select value="e">
                    <option @click="changeCateg(0)" value="e">Equipment</option>
                    <option @click="changeCateg(1)" value="u">User Consumable</option>
                    <option @click="changeCateg(2)" value="w">Waifu Consumable</option>
                    <option @click="changeCateg(3)" value="m">Materials</option>
                </select>
            </div>
            Equipment doesn't work right now
            <div v-for="item in items">
                
                <div v-if="item.type == filteredType">
                    <div class="itemDisplay">
                        <img class="waifuIcon" :src="`${publicPath}item-image/${item.imgPATH}`">
                        {{ item.id }} - {{ $t(`item.${item.id}.name`) }}
                        
                        <button class="smallbutton nnnbutton" @click="addItem(item)">Add</button>
                    </div>
                </div>
            </div>
            <button class="smallbutton nnnbutton" @click="UpdateDatabase()">Update</button>
            <!--<div class="itemDisplay">
                <img class="waifuIcon" :src="'src/assets/item-image/' + itemToModify.imgPATH">
                {{ itemToModify.name }}
                <input type="text">
                <button>Update in DB</button>
            </div>-->
        </div>
    </div>
</template>

<style lang="css" scoped>

#inventoryManager {
    display: grid;
    grid-template-columns: 1fr 1fr;
}
#theManager {
    padding-left: 1vw;
    border-left: blueviolet 5px solid;
}
#itemDisplay {
    display:grid;
    grid-template-columns: 1fr 1fr 1fr;
}
label {
    color:rgb(163, 115, 207);
}
input {
    margin-right: 1vw;
}

</style>