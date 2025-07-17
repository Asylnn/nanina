<script lang="ts">

export default {
    name : "PrivacyPage",

}


</script>
<template>
    <div class="flex margins">

        <h1> {{ $t(`privacy_policy.privacy_policy`) }}</h1>
        <h2> {{ $t(`privacy_policy.data_collection_title`) }} </h2>
        <p>  {{ $t(`privacy_policy.data_collection_body`) }} </p>
        <h2> {{ $t(`privacy_policy.use_of_data_title`) }} </h2>
        <p>  {{ $t(`privacy_policy.use_of_data_body`) }} </p>
        <h2> {{ $t(`privacy_policy.data_protection_title`) }} </h2>
        <p>  {{ $t(`privacy_policy.data_protection_body`) }} </p>
        <h2> {{ $t(`privacy_policy.cookies_title`) }} </h2>
        <p>  {{ $t(`privacy_policy.cookies_body`) }} </p>
        <h2> {{ $t(`privacy_policy.sharing_data_title`) }} </h2>
        <p>  {{ $t(`privacy_policy.sharing_data_body`) }} </p>
        <h2> {{ $t(`privacy_policy.your_rights_title`) }} </h2>
        <p>  {{ $t(`privacy_policy.your_rights_body`) }} </p>
        <h2> {{ $t(`privacy_policy.contact_us_title`) }} </h2>
        <p>  {{ $t(`privacy_policy.contact_us_body`) }} </p>

        <p>  {{ $t(`privacy_policy.last_updated`) }} </p>

    </div>
</template>

<style lang="css" scoped>

h1{
    text-align: center;
    margin-bottom: 40px;
}

li
{
    margin-left: 50px;
    margin-bottom: 8px;
}

p{
    padding-bottom: 30px;
}

img {
    width: 100%;
    height: 100%;
}
</style>