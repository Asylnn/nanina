<script lang="ts">

export default {
    name : "Homepage",
    props: {
        image : {
            type : String,
            required : true
        },
    }
}


</script>
<template>
    <div>
        <img :src="image">
    </div>
</template>

<style lang="css" scoped>
img {
    width: 100%;
    height: 100%;
}
</style>