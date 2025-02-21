<script lang="ts">
import type Equipment from '@/classes/item/equipment';
import User from '@/classes/user/user';
import ItemComponent from './ItemComponent/ItemComponent.vue';
import GridDisplayComponent from './GridDisplayComponent.vue';
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
        <div class="InventoryHeader">
            <div id="filterCateg">
                <select value="a">
                    <option @click="category = 'all'" value="a">{{$t("inventory.all")}}</option>
                    <option @click="changeTab('equipment')" value="e">{{$t("inventory.equipment")}}</option>
                    <option @click="changeTab('user_consumable')" value="u">{{$t("inventory.user_consumable")}}</option>
                    <option @click="changeTab('waifu_consumable')" value="w">{{$t("inventory.waifu_consumable")}}</option>
                    <option @click="changeTab('material')" value="m">{{$t("inventory.material")}}</option>
                </select>
            </div>
        </div>
        <div class="InventoryBody">
            <div v-if="category === 'equipment' || category === 'all'">
                <GridDisplayComponent :elements="user.inventory.equipment" tabindex="0" @keydown.esc="closeItemDisplay" @show-element="showItem" :columns=6></GridDisplayComponent>
                <div v-if="focusedView">
                    <div @click="closeItemDisplay" class="veil" ></div>
                    <ItemComponent :is-for-equiping="false" :item="item_to_display"></ItemComponent>
                </div>
                
            </div>
            <!--<div v-if="category === 'user_consumable' || category === 'all'">
                <span>User Consumable :</span><br>
                <div v-for="item in user.inventory.userConsumable">
                    <div class="itemDisplay">
                        <div class="waifuIcon">
                        <img :src="'src/assets/item-image/' + item.imgPATH">
                        </div>
                        <p>{{item.name}} + nombre : + {{item.count}}</p>
                    </div>
                </div>
            </div>
            <div v-if="category === 'waifu_consumable' || category === 'all'">
                <span>Waifu Consumable :</span><br>
                <div v-for="item in user.inventory.waifuConsumable">
                    <div class="itemDisplay">
                        <div class="waifuIcon">
                        <img :src="'src/assets/item-image/' + item.imgPATH">
                        </div>
                        <p>{{item.name}} + nombre : + {{item.count}}</p>
                    </div>
                </div>
            </div>-->
            <div v-if="category === 'material' || category === 'all'">
                <GridDisplayComponent :elements="user.inventory.material" @show-element="showItem" :columns=5></GridDisplayComponent>
                <div v-if="focusedView">
                    <div @click="closeItemDisplay" id="veil" ></div>
                    <ItemComponent  :is-for-equiping="false" @input="onEscape" :item="item_to_display" tabindex="0" @keydown.esc="closeItemDisplay"></ItemComponent>
                </div>
            </div>
        </div>
    </div>
    
</template>

<style lang="css" scoped>

span {
    color: blueviolet;
    font-size: x-large;
}

</style>