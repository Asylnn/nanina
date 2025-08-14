<script lang="ts">
import type Item from '@/classes/item/item';
import ItemType from '@/classes/item/item_type';
import StatModifier from '@/classes/modifiers/stat_modifier';
import User from '@/classes/user/user';
import StaticItemUseComponent from './StaticItemUseComponent.vue';


export default {
    name : "CherishMenu",
    data(){
        return {
            ItemType : ItemType,
            StatModifier: StatModifier,
            publicPath : import.meta.env.BASE_URL,
            itemsIds : [4,16] , //4 is cherry battery and 16 is one time energy regen
            items : [] as Array<Item>,
        }
    },
    props: {
        user : {
            type : User,
            required: true
        },
        itemDb : {
            type: Array<Item>,
            required : true,
        }
    },
    components:{
        StaticItemUseComponent
    },
    mounted()
    {
        this.items = this.itemDb.filter(item => this.itemsIds.includes(item.id))
    }

}


</script>
<template>
    <StaticItemUseComponent :user="user" :items="items">

    </StaticItemUseComponent>
</template>

<style lang="css" scoped>

</style>