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
            db : {} as Dictionary<Item>
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
    mounted() {
        this.db = Object.assign(this.item_db, this.equipment_db)
        
        
    },
    methods:{
        onClickChangePage(new_page:number){
            this.page = new_page
        },
        DeleteSet(id: number){
            delete this.set_db[id]
        },
        DeleteItem(id: number){
            delete this.db[id]
        },
        AddSet(){
            let set = new Set()
            let keys = Object.keys(this.set_db).sort((a, b) => +a - +b)
            set.id = +keys.slice(-1)[0] + 1 || 1 //Don't worry too much about it
            this.set_db[set.id] = set
        },
        AddItem(){
            let item = new Item()
            let keys = Object.keys(this.db).sort((a, b) => +a - +b)
            item.id = +keys.slice(-2)[0] +1 || 1 //Don't worry too much about it*/
            this.db[item.id] = item
        },
        UpdateDatabase(){
            //this.equipment = this.item_db.filter(item => item.type == ItemType.Equipment) as Equipment[]
            /*this.material = this.item_db.filter(item => item.type == ItemType.Material) as Item[]
            this.waifu_consumable = this.item_db.filter(item => item.type == ItemType.WaifuConsumable) as Item[]
            this.user_consumable = this.item_db.filter(item => item.type == ItemType.UserConsumable) as Item[]*/
            var new_item_db = {
                equipments:{} as Dictionary<Equipment>,
                items:{} as Dictionary<Item>,
            }
            for(let [key, value] of Object.entries(this.db))
            {
                if(value.type == ItemType.Equipment)
                    new_item_db.equipments[key] = value as Equipment
                else
                    new_item_db.items[key] = value 
            }
            this.SendToServer(ClientResponseType.UpdateItemDB, JSON.stringify(new_item_db), this.id)
            this.SendToServer(ClientResponseType.UpdateSetDB, JSON.stringify(this.set_db), this.id)
        }
    }
}


</script>
<template>
    <div class="InventoryHeader">
        <ul id="InventoryPages">
            <li @click="onClickChangePage(1)"><span>Item</span></li>
            <li @click="onClickChangePage(2)"><span>Set</span></li>
        </ul>
    </div>
    <div class="InventoryBody">
        <button @click="UpdateDatabase">update database</button>
        <div v-if="page == 1">
            <div>Item</div>
            
            <button @click="AddItem">Add Item</button>
            <div v-for="item in db">
                <ItemManagerComponent :item="item" @delete-item="DeleteItem"></ItemManagerComponent>
            </div>
        </div>
        <div v-else>
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