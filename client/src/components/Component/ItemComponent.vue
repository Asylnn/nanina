<script lang="ts">
import Equipment from '@/classes/item/equipment';
import Item from '@/classes/item/item';
import ItemType from '@/classes/item/item_type';
import ModifierComponent from './ModifierComponent.vue';

export default {
    name : "ItemComponent",
    data() {
        return {
            ItemType : ItemType,
            publicPath : import.meta.env.BASE_URL,
        }
    },
    props: {
        isForEquiping: {
            type : Boolean,
            required : true,
        },
        item: {
            type : [Item, Equipment],
            required : true
            
        },
    },
    components:{
        ModifierComponent,
    }
}

</script>

<template>
    <div :class="isForEquiping ? 'isForEquiping' : ''" id="focusedObject" >
        <div id="itemImage">
            <div><img :src="`${publicPath}/item-image/${item.imgPATH}`"></div>
        </div>
        <div id="ItemInfo">
            {{ $t(`item.${item.id}.name`) }}<br>
            {{ $t(`item.${item.id}.description`) }}<br><br>
            <p v-if="item.type == ItemType.Equipment">
                {{ $t(`item.type.type`) }} : {{ $t(`item.type.${(item as Equipment).piece}`) }}<br>
                {{ $t(`set.set`) }} : {{ $t(`set.${(item as Equipment).setId}.name`) }}<br><br>
            </p>
            <p>{{$t("modifiers.modifier")}} </p><br>
            <div v-for="modifier in item.modifiers">
                <ModifierComponent :modifier="modifier"></ModifierComponent>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

#focusedObject {
    position: fixed;
    /*min-width: 400px;
    min-height: 100px;
    width: 30vw;
    height: 15vh;*/
    border-radius: 15px;
    display: flex;
    flex-direction: row;
    padding: 2vh 2vh;
    z-index: 727;
    top:50%;                                /*Make the display be at the center of the screen*/
    left:50%;
    transform: translate(-50%, -50%);
    background-color: rgb(6, 16, 26);
}

#focusedObject img {
    /*max-width: 25vw;
    max-height: 60vh;*/
    padding-right: 5vw;
    height: 100px;
    width: 100px;
}

#itemImage {
    display: flex;
    flex-direction: row;
}

.isForEquiping {
    cursor:pointer;
}

</style>