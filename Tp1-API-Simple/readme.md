# TP 1 : Industrialisation & Sécurité Docker (Niveau Master)

##  Scénario : Migration "GMAO-Cloud"

Vous êtes ingénieur DevOps chez un éditeur de logiciel de **GMAO** (Gestion de Maintenance Assistée par Ordinateur). Dans le cadre d'un passage à une architecture de micro-services sous Kubernetes, vous devez dockeriser le module de diagnostic système.

L'objectif n'est pas seulement de faire "tourner" l'application, mais de garantir un niveau de **sécurité de classe entreprise** (Hardening) en respectant des contraintes strictes d'immuabilité et de moindres privilèges.

---

##  Objectifs Pédagogiques

* Maîtriser le **Multi-stage Build** pour optimiser la taille de l'image.
* Sécuriser l'exécution avec un **utilisateur non-root**.
* Mettre en œuvre l'**immuabilité** du FileSystem (`read-only`).
* Gérer les dépendances système spécifiques à **Alpine Linux**.
* Configurer l'**observabilité** via le Healthcheck natif de Docker.

---

## 🛠️ Pré-requis

* Docker Desktop (Windows/Mac) ou Docker Engine (Linux).
* Clonez ce dépôt :
```bash
git clone https://github.com/rolm0123456789/fyc-Docker.git
cd fyc-Docker/Tp1-API-Simple

```

---

##  Énoncé des Travaux

### 1. Construction du Dockerfile

Vous devez rédiger un `Dockerfile` optimisé. **Contraintes imposées :**

* **Fondation :** Utiliser `aspnet:10.0-alpine` pour le runtime final.
* **Build :** Utiliser `sdk:10.0-alpine` pour la phase de compilation.
* **Système :** Installer les packages `tzdata`, `icu-libs` et `curl` (nécessaires pour la timezone et le diagnostic).
* **Sécurité :** Ne pas utiliser l'utilisateur par défaut (root). Utiliser l'utilisateur `app`.
* **Performance :** Isoler le `dotnet restore` pour profiter du cache des layers Docker.

### 2. Durcissement du Conteneur (Runtime)

L'image doit être lancée avec les restrictions suivantes :

* Limitation de la mémoire vive à **256Mo**.
* Activation du mode **lecture seule** pour le système de fichiers.
* Configuration d'un **disque temporaire en RAM** sur `/tmp`.
* Injection du fuseau horaire `Europe/Paris`.

---

### 3. **Vérification :** Accédez à `http://localhost:8080`.

**Résultat attendu :**

```text
==========================================
🛠️  DOCKER PRO-READY DIAGNOSTIC
==========================================
[X] SECURITY : Running as Non-Root
[X] SECURITY : Immutable FileSystem (Read-only)
[X] CONFIG   : Timezone set to Europe/Paris
[X] RESOURCE : Memory Quota enforced (<500MB)
==========================================
RESULTAT    : PASSED
==========================================

```

---

## Livrables

Les étudiants doivent fournir :

1. Le fichier `Dockerfile`.
2. Le fichier `.dockerignore`.
3. La commande `docker run` complète.
4. Une capture d'écran du diagnostic affichant **PASSED**.

---

## Rappels de Sécurité (Tips)

* **Le principe de l'immuabilité :** Pourquoi utiliser `--read-only` ? Cela empêche l'injection de scripts malveillants sur le disque en cas de faille applicative.
* **Non-Root :** Un processus root dans un conteneur est une porte ouverte pour une "Container Escape" vers l'hôte.
* **Alpine Linux :** Distribution ultra-légère, mais attention : elle n'inclut pas les bibliothèques de globalisation par défaut (`icu-libs`).