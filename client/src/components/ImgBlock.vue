<script lang="ts">
import Page from '../classes/page';
export default {
    name : "ImgBlock",
    props: {
        imgURL : {
            type : String,
            required : true
        },
        name : {
            type : String,
            required : true
        }
    }
}

</script>

<template>
    <div id="imgBlock">
        <div>
            <p id="nameWaifu" style="margin-top: 0">{{ name }}</p>
            <img id="imgWaifu" :src="imgURL">
        </div>
    </div>
</template>

<style lang="css" scoped>
#imgBlock {
    width: 30%;
    float: left;
    border: 2px solid red;
}

#nameWaifu {
    margin: 0%;
    color:salmon;
    font: 30px cursive;
    text-decoration: underline;
    text-align: center;
}

img {
    height: auto;
    width: 50%;
    display: block;
    margin-left: auto;
    margin-right: auto;
}
</style>