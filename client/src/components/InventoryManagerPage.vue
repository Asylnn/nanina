<script lang="ts">
import type Equipment from '@/classes/inventory/equipment';
import type Material from '@/classes/inventory/material';
import type UserConsumable from '@/classes/inventory/user_consumable';
import type WaifuConsumable from '@/classes/inventory/waifu_consumable';
import InventoryPage from './InventoryPage.vue';
import User from '@/classes/user';
import type Item from '@/classes/inventory/item';
import ItemType from '@/classes/inventory/item_type';

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
            modeManager : "add"

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
        InventoryPage
    },
    methods: {
        changeCateg(categ : ItemType){
            this.categManager = categ
        },
        changeMode(mode : string) {
            this.modeManager = mode
        },
    },
}


</script>
<template>
    <div id="inventoryManager">
        <div id="theInventory">
            <InventoryPage :user="user"></InventoryPage>
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
            <div v-else-if="modeManager === 'update'">
                <div v-for="dico in user.inventory">
                    <div v-for="item in dico">
                        <div v-if="item.type == categManager">
                            <div class="itemDisplay">
                                <img class="waifuIcon" :src="'src/assets/item-image/' + item.imgPATH">
                                {{ item.name }}
                                <button>Update</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div v-else><!--modeManager === delete-->
                <div v-for="dico in user.inventory">
                    <div v-for="item in dico">
                        <div v-if="item.type == categManager">
                            <div class="itemDisplay">
                                <img class="waifuIcon" :src="'src/assets/item-image/' + item.imgPATH">
                                {{ item.name }}
                                <input type="text">
                                <button>Delete</button>
                            </div>
                        </div>
                    </div>
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