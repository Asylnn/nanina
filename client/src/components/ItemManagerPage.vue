<script lang="ts">
import type Item from '@/classes/inventory/item';
import Equipment from '@/classes/inventory/equipment';
import EquipmentManagerComponent from './ItemManagerComponent/EquipmentManagerComponent.vue'
import ItemType from '@/classes/inventory/item_type';
import UserConsumable from '@/classes/inventory/user_consumable';
import WaifuConsumable from '@/classes/inventory/waifu_consumable';
import Material from '@/classes/inventory/material';
import MaterialManagerComponent from './ItemManagerComponent/MaterialManagerComponent.vue';
import UserConsumableManagerComponent from './ItemManagerComponent/UserConsumableManagerComponent.vue';
import WaifuConsumableManagerComponent from './ItemManagerComponent/WaifuConsumableManagerComponent.vue';

export default {
    name : "ItemManagerPage",
    data() {
        return {
            page: 1,
            equipment: new Array<Equipment>(),
            user_consumable: new Array<UserConsumable>(),
            waifu_consumable: new Array<WaifuConsumable>(),
            material: new Array<Material>(),
        }
    },
    props:{ 
        item_db :{
            type: Array<Item>,
            required : true,
        },
        id : {
            type: String,
            required : true,
        }
        
    },
    components:{
        EquipmentManagerComponent,
        MaterialManagerComponent,
        UserConsumableManagerComponent,
        WaifuConsumableManagerComponent,
    },
    mounted() {
        this.equipment = this.item_db.filter(item => item.type == ItemType.Equipment) as Equipment[]
        this.material = this.item_db.filter(item => item.type == ItemType.Material) as Material[]
        this.waifu_consumable = this.item_db.filter(item => item.type == ItemType.WaifuConsumable) as WaifuConsumable[]
        this.user_consumable = this.item_db.filter(item => item.type == ItemType.UserConsumable) as UserConsumable[]
    },
    methods:{
        onClickChangePage(new_page:number){
            this.page = new_page
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
        AddEquipment(){
            this.equipment.push(new Equipment())
        },
        AddUserConsumable(){
            this.user_consumable.push(new UserConsumable())
        },
        AddWaifuConsumable(){
            this.waifu_consumable.push(new WaifuConsumable())
        },
        AddMaterial(){
            this.material.push(new Material())
        },
        UpdateDatabase(){
            var new_item_db = {
                equipment:this.equipment,
                material:this.material,
                waifu_consumable:this.waifu_consumable,
                user_consumable:this.user_consumable,
            }
            //@ts-ignore
            this.ws.send(JSON.stringify({type:"update item db", data:JSON.stringify(new_item_db), id: this.id}))
        }
    }
}


</script>
<template>
    <div class="InventoryHeader">
        <ul id="InventoryPages">
            <li @click="onClickChangePage(1)"><span>Equipment</span></li>
            <li @click="onClickChangePage(2)"><span>User Consumables</span></li>
            <li @click="onClickChangePage(3)"><span>Waifu Consumables</span></li>
            <li @click="onClickChangePage(4)"><span>Materials</span></li>
        </ul>
    </div>
    <div class="InventoryBody">
        <button @click="UpdateDatabase">update database</button>
        <div v-if="page == 1">
            <div>Equipement</div>
            
            <button @click="AddEquipment">add equipment</button>
            <div v-for="item in equipment">
                <EquipmentManagerComponent :item="item" @delete-item="DeleteEquipement"></EquipmentManagerComponent>
            </div>
        </div>
        <div v-else-if="page == 2">
            <div>User Consumables</div>
            <button @click="AddUserConsumable">add user consumable</button>
            <div v-for="item in user_consumable">
                <UserConsumableManagerComponent :item="item" @delete-item="DeleteUserConsumable"></UserConsumableManagerComponent>
            </div>
        </div>
        <div v-else-if="page == 3">
            <div>Waifu Consumables</div>
            <button @click="AddWaifuConsumable">add waifu consumable</button>
            <div v-for="item in waifu_consumable">
                <WaifuConsumableManagerComponent :item="item" @delete-item="DeleteWaifuConsumable"></WaifuConsumableManagerComponent>
            </div>
        </div>
        <div v-else>
            <div>Materials</div>
            <button @click="AddMaterial">add material</button>
            <div v-for="item in material">
                <MaterialManagerComponent :item="item" @delete-item="DeleteMaterial"></MaterialManagerComponent>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>


.InventoryHeader{
    align-content: center;
}

#InventoryPages {
    display:grid;
    cursor: pointer;
    text-align: center;
    grid-template-columns: 1fr 1fr 1fr 1fr ;
}
.img{
    max-width: 32px;
}

</style>