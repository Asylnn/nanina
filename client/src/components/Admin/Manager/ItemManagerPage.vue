<script lang="ts">
import Item from '@/classes/item/item';
import Equipment from '@/classes/item/equipment';
import ItemType from '@/classes/item/item_type';
import ItemManagerComponent from './ItemManagerComponent.vue';
import SetManagerComponent from './SetManagerComponent.vue';
import Set from '@/classes/item/set'
import ClientResponseType from '@/classes/client_response_type';
import type Dictionary from '@/classes/dictionary';
import type { PropType } from 'vue';

export default {
    name : "ItemManagerPage",
    data() {
        return {
            page: 1,
            maxID: 1,
        }
    },
    props:{ 
        item_db :{
            type: Object as PropType<Dictionary<Item>>,
            required : true,
        },
        equipment_db:{
            type: Object as PropType<Dictionary<Equipment>>,
            required : true,
        },
        set_db: {
            type: Object as PropType<Dictionary<Set>>,
            required:true,
        },
        id : {
            type: String,
            required : true,
        }
    },
    components:{
        ItemManagerComponent,
        SetManagerComponent,
    },
    methods:{
        GetId() : number
        {
            let db = Object.assign(this.item_db, this.equipment_db)
            let ids = Object.keys(db).sort((a, b) => +a - +b) 
            return +ids.slice(-2)[0] + 1 || 1
        },
        DeleteSet(id: number){
            delete this.set_db[id]
        },
        DeleteItem(id: number){
            delete this.item_db[id]
        },
        DeleteEquipment(id : number)
        {
            delete this.item_db[id]
        },
        AddSet(){
            let set = new Set()
            let keys = Object.keys(this.set_db).sort((a, b) => +a - +b)
            set.id = +keys.slice(-1)[0] + 1 || 1 //Don't worry too much about it
            this.set_db[set.id] = set
        },
        AddItem(){
            let item = new Item()
            item.id = this.GetId()
            this.item_db[item.id] = item
        },
        AddEquipment()
        {
            let item = new Equipment()
            item.id = this.GetId()
            this.equipment_db[item.id] = item
        },
        UpdateDatabase(){
            //this.equipment = this.item_db.filter(item => item.type == ItemType.Equipment) as Equipment[]
            /*this.material = this.item_db.filter(item => item.type == ItemType.Material) as Item[]
            this.waifu_consumable = this.item_db.filter(item => item.type == ItemType.WaifuConsumable) as Item[]
            this.user_consumable = this.item_db.filter(item => item.type == ItemType.UserConsumable) as Item[]*/
            var new_item_db = {
                equipments: this.equipment_db,
                items: this.item_db,
            }
            /*for(let [key, value] of Object.entries(this.db))
            {
                if(value.type == ItemType.Equipment)
                    new_item_db.equipments[key] = value as Equipment
                else
                    new_item_db.items[key] = value 
            }*/
            this.SendToServer(ClientResponseType.UpdateItemDB, JSON.stringify(new_item_db), this.id)
            this.SendToServer(ClientResponseType.UpdateSetDB, JSON.stringify(this.set_db), this.id)
        }
    }
}


</script>
<template>
    <div class="InventoryHeader">
        <ul id="InventoryPages">
            <li @click="page = 1"><span>Material</span></li>
            <li @click="page = 4"><span>Equipment</span></li>
            <!--<li @click="page = 2"><span>User Consumable</span></li>
            <li @click="page = 3"><span>Waifu Consumable</span></li>
            
            <li @click="page = 5"><span>Duplicates</span></li>-->
            <li @click="page = 6"><span>Set</span></li>
        </ul>
    </div>
    <div class="InventoryBody">
        <button @click="UpdateDatabase">update database</button>
        <div v-if="page == 1">
            <div>Item</div>
            
            <button @click="AddItem">Add Item</button>
            <div v-for="item in item_db">
                <ItemManagerComponent :item="item" @delete-item="DeleteItem"></ItemManagerComponent>
            </div>
        </div>
        <div v-if="page == 4">
            <div>Item</div>
            
            <button @click="AddEquipment">Add Equipment</button>
            <div v-for="item in equipment_db">
                <ItemManagerComponent :item="item" @delete-item="DeleteEquipment"></ItemManagerComponent>
            </div>
        </div>
        
        <div v-if="page == 6">
            <div>Set</div>
            <button @click="AddSet">Add Set</button>
            <div v-for="set in set_db">
                <SetManagerComponent :set="set" @delete-item="DeleteSet"></SetManagerComponent>
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