<script lang="ts">
import Equipment from '@/classes/item/equipment';
import Item from '@/classes/item/item';
import ItemType from '@/classes/item/item_type';


export default {
    name : "ItemComponent",
    data() {
        return {
            ItemType : ItemType,
        }
    },
    props: {
        item: {
            type : [Item, Equipment],
            required : true
            
        },
    },
}

</script>

<template>
    <div id="focusedObject" >
        <div id="waifuPic"><img :src="'src/assets/item-image/' + item.imgPATH"></div>
        <div id="ItemInfo">
            {{ $t(`item.${item.id}.name`) }}<br>
            {{ $t(`item.${item.id}.description`) }}<br>
            <p v-if="item.type == ItemType.Equipment">
                {{ $t(`item.type.type`) }} : {{ $t(`item.type.${item.type}`) }}<br>
                {{ $t(`set.set`) }} : {{ $t(`set.${(item as Equipment).setId}.name`) }}<br>
            </p>
        </div>
    </div>
</template>

<style lang="css" scoped>

#focusedObject {
    position: fixed;
    width: 40vw;
    height: 50vh;
    border-radius: 15px;
    display: grid;
    grid-template-columns: 1fr 1fr;
    padding: 2vh 2vh;
    z-index: 727;
    top: 25vh; 
    left: 30vw;
    background-color: rgb(6, 16, 26);
}
#focusedObject img {
    max-width: 25vw;
    max-height: 60vh;
}

#waifuPic {
    border-radius: 15px;
    overflow: hidden;
}
#waifuInfos {
    padding: 0 1vw;
}

</style>