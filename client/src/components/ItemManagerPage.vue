<script lang="ts">

export default {
    name : "ItemManagerPage",
    data() {
        return {
            page: 1,
        }
    },
    props:{
        item_db : Array<Item>,
        required : true,
    },
    methods:{
        onClickChangePage(new_page:number){
            this.page = new_page
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
        <div v-if="page == 1">
            <div>Equipement</div>
            <div v-for="item in item_db">
                <button @click="Delete">delete</button>
                <span class="attribute"><input class="name" v-model="waifu.name" type="text"></span>
                <span class="attribute">Id  <input class="numberInput" v-model="waifu.id" type="text"></span>
                <span class="attribute">Diff  <input class="numberInput" v-model="waifu.diffLvlUp" type="number"></span>
                <span class="attribute">★  <input class="numberInput" v-model="waifu.stars" type="number"></span>
                
                <span class="attribute">_STR  <input class="numberInput" v-model="waifu.o_str" type="number"></span>
                <span class="attribute">+STR  <input class="numberInput" v-model="waifu.u_str" type="number"></span>
                <span class="attribute">_AGI  <input class="numberInput" v-model="waifu.o_agi" type="number"></span>
                <span class="attribute">+AGI  <input class="numberInput" v-model="waifu.u_agi" type="number"></span>
                <span class="attribute">_KAW  <input class="numberInput" v-model="waifu.o_kaw" type="number"></span>
                <span class="attribute">+KAW  <input class="numberInput" v-model="waifu.u_kaw" type="number"></span>
                <span class="attribute">_DEX  <input class="numberInput" v-model="waifu.o_dex" type="number"></span>
                <span class="attribute">+DEX  <input class="numberInput" v-model="waifu.u_dex" type="number"></span>
                <span class="attribute">_INT  <input class="numberInput" v-model="waifu.o_int" type="number"></span>
                <span class="attribute">+INT  <input class="numberInput" v-model="waifu.u_int" type="number"></span>
                <span class="attribute">_LUCK  <input class="numberInput" v-model="waifu.o_luck" type="number"></span>
                <span class="attribute">+LUCK  <input class="numberInput" v-model="waifu.u_luck" type="number"></span>

                <span class="attribute">Img  <input class="imgImput"v-model="waifu.imgPATH" type="text"></span>
            </div>
        </div>
        <div v-else-if="page == 2">
            <div>USer Consumables</div>
        </div>
        <div v-else-if="page == 3">
            <div>Waifu Consumables</div>
        </div>
        <div v-else>
            <div>Materials</div>
        </div>
    </div>
</template>

<style lang="css" scoped>
.InventoryHeader{
    align-content: center;
}

img {
    width: 100%;
    height: 100%;
}

#InventoryPages {
    display:grid;
    cursor: pointer;
    text-align: center;
    grid-template-columns: 1fr 1fr 1fr 1fr ;
}


</style>