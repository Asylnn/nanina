<script lang="ts">
import type Equipment from '@/classes/item/equipment';
import User from '@/classes/user/user';
import ItemComponent from '../Component/ItemComponent.vue';
import GridDisplayComponent from '../Component/GridDisplayComponent.vue';
import Item from '@/classes/item/item';
import ItemType from '@/classes/item/item_type';

export default {
    name : "InventoryPage",
    data(){
        return {
            category : "all",
            focusedView : false,
            item_to_display : new Item(),
            ItemType:ItemType,
        }
    },
    props: {
        user: {
            type: User,
            required: true
        }
    },
    methods:{
        closeItemDisplay(){
            this.focusedView = false
            this.item_to_display = new Item()
        },
        showItem(item : Item){
            this.focusedView = true
            this.item_to_display = item
            
        },
        onEscape(){

        },
        applyTextColor(categoryName : string){
            var style = "";
            if(categoryName == this.category)
                style = "color: blueviolet;";
            return style
        }
    },
    components:{
        ItemComponent,
        GridDisplayComponent,
    }
}


</script>
<template>
    <div id="inventory">
        <ul id="inventoryHeader">
            <li :style="applyTextColor('all')" @click="category = 'all'">{{$t("inventory.all")}}</li>
            <li :style="applyTextColor('equipment')" @click="category = 'equipment'">{{$t("inventory.equipment")}}</li>
            <li :style="applyTextColor('user_consumable')" @click="category = 'user_consumable'">{{$t("inventory.user_consumable")}}</li>
            <li :style="applyTextColor('waifu_consumable')" @click="category = 'waifu_consumable'">{{$t("inventory.waifu_consumable")}}</li>
            <li :style="applyTextColor('material')" @click="category = 'material'" >{{$t("inventory.material")}}</li>
        </ul>
        <div id="inventoryBody">
            <GridDisplayComponent v-if="category === 'equipment' || category === 'all'" 
                :elements="Object.values(user.inventory.equipment)" 
                tabindex="0" 
                @keydown.esc="closeItemDisplay" 
                @show-element="showItem" 
                :columns=8>
            </GridDisplayComponent>
            <GridDisplayComponent v-if="category === 'user_consumable' || category === 'all'" 
                tabindex="0" 
                @keydown.esc="closeItemDisplay" 
                @show-element="showItem" :columns=8
                :elements="Object.values(user.inventory.items).filter(item => item.type == ItemType.UserConsumable)">

            </GridDisplayComponent>
            <GridDisplayComponent v-if="category === 'waifu_consumable' || category === 'all'" 
                :elements="Object.values(user.inventory.items).filter(item => item.type == ItemType.WaifuConsumable)" 
                tabindex="0" 
                @keydown.esc="closeItemDisplay" 
                @show-element="showItem" 
                :columns=8>
            </GridDisplayComponent>
            <GridDisplayComponent v-if="category === 'material' || category === 'all'" 
                :elements="Object.values(user.inventory.items).filter(item => item.type == ItemType.Material)" 
                tabindex="0" 
                @keydown.esc="closeItemDisplay" 
                @show-element="showItem" 
                :columns=8>
            </GridDisplayComponent>
            <div v-if="focusedView">
                <div @click="closeItemDisplay" class="veil" ></div>
                <ItemComponent :userID="user.Id" :is-for-equiping="false" @exit="closeItemDisplay" :item="item_to_display"></ItemComponent>
            </div>
        </div>
    </div>
    
</template>

<style lang="css" scoped>

#inventoryHeader {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
    height: 5vh;
    margin: 0 20vw;
    text-align: center;
    cursor: pointer;
}

</style>