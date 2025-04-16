<script lang="ts">
import Item from '@/classes/item/item';
import Equipment from '@/classes/item/equipment';
import ItemType from '@/classes/item/item_type';
import ItemManagerComponent from './ItemManagerComponent.vue';
import SetManagerComponent from './SetManagerComponent.vue';
import Set from '@/classes/item/set'

export default {
    name : "ItemManagerPage",
    data() {
        return {
            page: 1,
        }
    },
    props:{ 
        item_db :{
            type: Array<Item>,
            required : true,
        },
        set_db: {
            type: Array<Set>,
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
        
        
        
    },
    methods:{
        onClickChangePage(new_page:number){
            this.page = new_page
        },
        DeleteSet(id: number){
            this.set_db.splice(this.set_db.findIndex(set => set.id == id), 1)
        },
        DeleteItem(id: number){
            this.item_db.splice(this.item_db.findIndex(item => item.id == id), 1)
        },
        AddSet(){
            this.set_db.push(new Set())
        },
        AddItem(){
            this.item_db.push(new Item())
        },
        UpdateDatabase(){
            //this.equipment = this.item_db.filter(item => item.type == ItemType.Equipment) as Equipment[]
            /*this.material = this.item_db.filter(item => item.type == ItemType.Material) as Item[]
            this.waifu_consumable = this.item_db.filter(item => item.type == ItemType.WaifuConsumable) as Item[]
            this.user_consumable = this.item_db.filter(item => item.type == ItemType.UserConsumable) as Item[]*/
            var new_item_db = {
                equipment:this.item_db.filter(item => item.type == ItemType.Equipment),
                material:this.item_db.filter(item => item.type == ItemType.Material),
                waifu_consumable:this.item_db.filter(item => item.type == ItemType.WaifuConsumable),
                user_consumable:this.item_db.filter(item => item.type == ItemType.UserConsumable),
            }
            this.SendToServer("update item db", JSON.stringify(new_item_db), this.id)
            this.SendToServer("update set db", JSON.stringify(this.set_db), this.id)
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
            <div v-for="item in item_db">
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