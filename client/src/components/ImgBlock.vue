<script lang="ts">

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