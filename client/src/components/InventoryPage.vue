<script lang="ts">
import type Equipment from '@/classes/item/equipment';
import User from '@/classes/user/user';


export default {
    name : "InventoryPage",
    data(){
        return {
            category : "all"
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
        }
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
                    <option @click="changeTab('all')" value="a">All</option>
                    <option @click="changeTab('equipment')" value="e">Equipement</option>
                    <option @click="changeTab('user_consumable')" value="u">User Consumable</option>
                    <option @click="changeTab('waifu_consumable')" value="w">Waifu Consumable</option>
                    <option @click="changeTab('materials')" value="m">Materials</option>
                </select>
            </div>
        </div>
        <div class="InventoryBody">
            <div v-if="category === 'equipment' || category === 'all'">
                <span>Equipement :</span><br>
                <div v-for="item in user.inventory.equipment">
                    <div class="itemDisplay">
                        <div class="waifuIcon">
                        <img :src="'src/assets/item-image/' + item.imgPATH">
                        </div>
                        <p>{{item.name}}</p>
                    </div>
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
                <div v-for="item in user.inventory.material">
                    <div class="itemDisplay">
                        <div class="waifuIcon">
                        <img :src="'src/assets/item-image/' + item.imgPATH">
                        </div>
                        <p>{{item.name}} + nombre : + {{item.count}}</p>
                    </div>
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