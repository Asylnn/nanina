<script lang="ts">

export default {
    name : "DisplayComponent",
    data() {
        return {
        }
    },
    props: {
        type:{
            type:String,
            requied:true,
        }
    },
    emits:["click", "exit"],
    methods:{
        getZIndex()
        {
            switch(this.type)
            {
                case "waifu":
                    return "Z100"
                case "item":
                    return "Z200"
            }
        }
    }
}

</script>

<template>
    <div id="display" :class="getZIndex()">
        <button id="closeButton" @click="$emit('exit')"><img src="@/assets/x.png"></button>
        <slot></slot>
    </div>
</template>

<style lang="css" scoped>


#display {
    position: fixed;
    display: flex;
    flex-direction: row;
    border-radius: 15px;
    padding: 2vh 2vh;
    
    top:50%;                                /*Make the display be at the center of the screen*/
    left:50%;
    transform: translate(-50%, -50%);
    background-color: rgb(6, 16, 26);
}

.Z100
{
    z-index: 100;
}

.Z200
{
    z-index: 200;
}

#closeButton
{
    position:fixed;
    left:94%
}

#closeButton img
{
    width:26px;
    height:26px;
    filter: invert(0.8);
}

#closeButton img:hover
{
    filter: invert(1);
}
</style>