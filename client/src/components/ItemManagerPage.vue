<script lang="ts">

export default {
    name : "ItemManagerPage",
    data() {
        return {
            page: 1,
        }
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