<script lang="ts">

export default {
    name : "Homepage",
    props: {
        image : {
            type : String,
            required : true
        },
    },
    methods:{
        onClickChangePage(page:number){

        }
    }
}


</script>
<template>
    <div class="InventoryHeader">
        <ul id="pages">
            <li @click="onClickChangePage(1)"><span>Equipment</span></li>
            <li @click="onClickChangePage(2)"><span>User Consumables</span></li>
            <li @click="onClickChangePage(3)"><span>Waifu Consumables</span></li>
            <li @click="onClickChangePage(4)"><span>Materials</span></li>
        </ul>
    </div>
    <div class="InventoryBody">

    </div>
</template>

<style lang="css" scoped>
img {
    width: 100%;
    height: 100%;
}
</style>