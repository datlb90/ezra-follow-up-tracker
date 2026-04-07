<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

import AppLayout from '@/components/layout/AppLayout.vue'

const route = useRoute()
const isAuthPage = computed(() => route.meta.isAuthPage === true)
</script>

<template>
  <AppLayout v-if="!isAuthPage" />
  <RouterView v-else />
</template>
