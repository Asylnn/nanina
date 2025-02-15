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
        User : {{ user.username }}<br>
        Categ choisie : {{ category }}
        <div class="InventoryHeader">
            <div id="filterCateg">
                <label>Categ : </label>
                <select value="a">
                    <option @click="category = 'all'" value="a">All</option>
                    <option @click="changeTab('equipment')" value="e">Equipement</option>
                    <option @click="changeTab('user_consumable')" value="u">User Consumable</option>
                    <option @click="changeTab('waifu_consumable')" value="w">Waifu Consumable</option>
                    <option @click="changeTab('materials')" value="m">Materials</option>
                </select>
            </div>
        </div>
        <div class="InventoryBody">
            <div v-if="category === 'equipment' || category === 'all'">
                <GridDisplayComponent :elements="user.inventory.equipment" @show-element="showItem" :columns=5></GridDisplayComponent>
                <div v-if="focusedView">
                    <div @click="closeItemDisplay" id="veil" ></div>
                    <ItemComponent  @input="onEscape" :item="item_to_display" tabindex="0" @keydown.esc="closeItemDisplay"></ItemComponent>
                </div>
                
            </div>
            <div v-if="category === 'user_consumable' || category === 'all'">
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
            </div>
            <div v-if="category === 'materials' || category === 'all'">
                <span>Materials :</span><br>
                <GridDisplayComponent :elements="user.inventory.material" @show-element="showItem" :columns=5></GridDisplayComponent>
                <div v-if="focusedView">
                    <div @click="closeItemDisplay" id="veil" ></div>
                    <ItemComponent  @input="onEscape" :item="item_to_display" tabindex="0" @keydown.esc="closeItemDisplay"></ItemComponent>
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