<script lang="ts">
import Item from '@/classes/item/item';


export default {
    name : "ItemGridComponent",
    data() {
        return {
        }
    },
    props: {
        items: {
            type : Array<Item>,
            required : true
        },
        columns: { //grid-template-columns
            type : Number,
            required : true
        },
    },
    computed: { //omg a computed that actually works??????????
        generateGridTemplateColumns() {
            var gridTemplateColumns = "grid-template-columns: ";
            for (let i = 0; i < this.columns; i++) {
                gridTemplateColumns += "1fr ";
            }
            gridTemplateColumns += '; ';
            return gridTemplateColumns
        },
    },
    emits: ["show-item"],
    methods: {
        showItem(item : Item) {
            this.$emit("show-item", item)
        },
    },
}

</script>

<template>
    <div id="waifuIcons" :style=generateGridTemplateColumns>
        <div v-for="item in items">
            <div class="waifuDisplay">
                <div class="waifuIcon">
                    <img @click="showItem(item)" :src="'src/assets/item-image/' + item.imgPATH">
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>
#waifuIcons {
    padding: 0 17.27vw;
    position:relative;
}
#waifuIcons {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
}
.waifuDisplay {
    margin: 2vh 2vw;
    width: 10vw;
}
.waifuIcon {
    border-radius: 15px;
    max-width: 10vw;
    max-height: 20vh;
    overflow: hidden;
}
.waifuIcon img {
    max-width: 15vw;
    max-height: 35vh;
    cursor: pointer;
}
.waifuDisplay p {
    text-align: center;
}
</style>