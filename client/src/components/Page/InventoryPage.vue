<script lang="ts">
import type Equipment from '@/classes/item/equipment';
import User from '@/classes/user/user';
import ItemComponent from '../Component/ItemComponent.vue';
import GridDisplayComponent from '../Component/GridDisplayComponent.vue';
import Item from '@/classes/item/item';

export default {
    name : "InventoryPage",
    data(){
        return {
            category : "all",
            focusedView : false,
            item_to_display : new Item()
        }
    },
    props: {
        user: {
            type: User,
            required: true
        }
    },
    methods:{
        changeTab(categ : string){
            this.category = categ
        },
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
            <li :style="applyTextColor('all')" @click="changeTab('all')">{{$t("inventory.all")}}</li>
            <li :style="applyTextColor('equipment')" @click="changeTab('equipment')">{{$t("inventory.equipment")}}</li>
            <li :style="applyTextColor('user_consumable')" @click="changeTab('user_consumable')">{{$t("inventory.user_consumable")}}</li>
            <li :style="applyTextColor('waifu_consumable')" @click="changeTab('waifu_consumable')">{{$t("inventory.waifu_consumable")}}</li>
            <li :style="applyTextColor('material')" @click="changeTab('material')" >{{$t("inventory.material")}}</li>
        </ul>
        <div id="inventoryBody">
            <div v-if="category === 'equipment' || category === 'all'">
                <GridDisplayComponent :elements="user.inventory.equipment" tabindex="0" @keydown.esc="closeItemDisplay" @show-element="showItem" :columns=8></GridDisplayComponent>
                <div v-if="focusedView">
                    <div @click="closeItemDisplay" class="veil" ></div>
                    <ItemComponent :userID="user.Id" :is-for-equiping="false" :item="item_to_display"></ItemComponent>
                </div>
            </div>
            <div v-if="category === 'user_consumable' || category === 'all'">
                <GridDisplayComponent :elements="user.inventory.userConsumable" tabindex="0" @keydown.esc="closeItemDisplay" @show-element="showItem" :columns=8></GridDisplayComponent>
                <div v-if="focusedView">
                    <div @click="closeItemDisplay" class="veil" ></div>
                    <ItemComponent :userID="user.Id" :is-for-equiping="false" :item="item_to_display"></ItemComponent>
                </div>
            </div>
            <div v-if="category === 'waifu_consumable' || category === 'all'">
                <GridDisplayComponent :elements="user.inventory.waifuConsumable" tabindex="0" @keydown.esc="closeItemDisplay" @show-element="showItem" :columns=8></GridDisplayComponent>
                <div v-if="focusedView">
                    <div @click="closeItemDisplay" class="veil" ></div>
                    <ItemComponent :userID="user.Id" :is-for-equiping="false" :item="item_to_display"></ItemComponent>
                </div>
            </div>
            <div v-if="category === 'material' || category === 'all'">
                <GridDisplayComponent :elements="user.inventory.material" tabindex="0" @keydown.esc="closeItemDisplay" @show-element="showItem" :columns=8></GridDisplayComponent>
                <div v-if="focusedView">
                    <div @click="closeItemDisplay" class="veil" ></div>
                    <ItemComponent :userID="user.Id" :is-for-equiping="false" @input="onEscape" :item="item_to_display" tabindex="0" @keydown.esc="closeItemDisplay"></ItemComponent>
                </div>
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