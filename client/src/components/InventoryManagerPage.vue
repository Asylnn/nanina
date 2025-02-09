<script lang="ts">
import type Equipment from '@/classes/inventory/equipment';
import type Material from '@/classes/inventory/material';
import type UserConsumable from '@/classes/inventory/user_consumable';
import type WaifuConsumable from '@/classes/inventory/waifu_consumable';
import InventoryPage from './InventoryPage.vue';
import User from '@/classes/user';
import Item from '@/classes/inventory/item';
import ItemType from '@/classes/inventory/item_type';
import EquipmentManagerComponent from './ItemManagerComponent/EquipmentManagerComponent.vue';
import UserConsumableManagerComponent from './ItemManagerComponent/UserConsumableManagerComponent.vue';
import WaifuConsumableManagerComponent from './ItemManagerComponent/WaifuConsumableManagerComponent.vue';
import MaterialManagerComponent from './ItemManagerComponent/MaterialManagerComponent.vue';

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
            categManager : ItemType.Equipment,
            modeManager : "add",
            itemToModify : null as Item | null,
            equipment: new Array<Equipment>(),
            user_consumable: new Array<UserConsumable>(),
            waifu_consumable: new Array<WaifuConsumable>(),
            material: new Array<Material>(),
        }
    },
    props : {
        user: {
            type: User,
            required: true
        },
        items: { //type dans l'ordre du select
            type: Array<Item>,
            required: true
        },
    },
    components: {
        InventoryPage,
        EquipmentManagerComponent,
        UserConsumableManagerComponent,
        WaifuConsumableManagerComponent,
        MaterialManagerComponent,
    },
    methods: {
        changeCateg(categ : ItemType){
            this.categManager = categ
        },
        changeMode(mode : string) {
            this.modeManager = mode
        },
        showItemToModify(item : Item) {
            this.itemToModify = item
        },

        DeleteEquipement(id: number){
            this.equipment.splice(this.equipment.findIndex(equipment => equipment.id == id), 1)
        },
        DeleteMaterial(id: number){
            this.material.splice(this.material.findIndex(material => material.id == id), 1)
        },
        DeleteWaifuConsumable(id: number){
            this.waifu_consumable.splice(this.waifu_consumable.findIndex(waifu_consumable => waifu_consumable.id == id), 1)
        },
        DeleteUserConsumable(id: number){
            this.user_consumable.splice(this.user_consumable.findIndex(user_consumable => user_consumable.id == id), 1)
        },

        UpdateDatabase(){
            var new_item_db = {
                equipment:this.equipment,
                material:this.material,
                waifu_consumable:this.waifu_consumable,
                user_consumable:this.user_consumable,
            }
            this.ws.send(JSON.stringify({type:"update item db", data:JSON.stringify(this.user.inventory), id: this.user.Id}))
        }
    },
}


</script>
<template>
    <div id="inventoryManager">
        <div id="theInventory">
            <div v-if="modeManager === 'add'">
                <InventoryPage :user="user"></InventoryPage>
            </div>
            <div v-else>
                <div v-for="dico in user.inventory">
                    <div v-for="item in dico">
                        <div v-if="item.type == ItemType.Equipment">
                                <EquipmentManagerComponent :item="item" @delete-item="DeleteEquipement"></EquipmentManagerComponent>
                        </div>
                        <div v-else-if="item.type == ItemType.UserConsumable">
                            <UserConsumableManagerComponent :item="item" @delete-item="DeleteUserConsumable"></UserConsumableManagerComponent>
                        </div>
                        <div v-else-if="item.type == ItemType.WaifuConsumable">
                            <WaifuConsumableManagerComponent :item="item" @delete-item="DeleteWaifuConsumable"></WaifuConsumableManagerComponent>
                        </div>
                        <div v-else>
                            <MaterialManagerComponent :item="item" @delete-item="DeleteMaterial"></MaterialManagerComponent>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="theManager">
            Bienvenue a l'inventaire manager<br>
            <div id="filterModeManager">
                <label>Mode : </label>
                <select value="a">
                    <option @click="changeMode('add')" value="a">Add</option>
                    <option @click="changeMode('update')" value="u">Update</option>
                    <option @click="changeMode('delete')" value="d">Delete</option>
                </select>
                {{ modeManager }}
            </div>
            <div id="filterCategManager">
                <label>Categ : </label>
                <select value="e">
                    <option @click="changeCateg(0)" value="e">Equipment</option>
                    <option @click="changeCateg(1)" value="u">User Consumable</option>
                    <option @click="changeCateg(2)" value="w">Waifu Consumable</option>
                    <option @click="changeCateg(3)" value="m">Materials</option>
                </select>
                {{ categManager }}
            </div>
            <div v-if="itemToModify == null">
                <div v-if="modeManager === 'add'">
                    <div v-for="item in items">
                        <div v-if="item.type == categManager">
                            <div class="itemDisplay">
                                <img class="waifuIcon" :src="'src/assets/item-image/' + item.imgPATH">
                                {{ item.name }}
                                <label>Quantité : </label>
                                <input type="number">
                                <button>Add</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div v-else>
                <div class="itemDisplay">
                    <img class="waifuIcon" :src="'src/assets/item-image/' + itemToModify.imgPATH">
                    {{ itemToModify.name }}
                    <input type="text">
                    <button>Update in DB</button>
                </div>
            </div>
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