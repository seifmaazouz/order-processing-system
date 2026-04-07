# 1.0.0 (2026-01-05)

## Features
* feat: implement complete semantic-release CI/CD pipeline (#63) ([de34c4f](https://github.com/seifmaazouz/order-processing-system/commit/de34c4f))
* feat: add shopping cart and credit card endpoints (#48) ([c4cd4d6](https://github.com/seifmaazouz/order-processing-system/commit/c4cd4d6))
* feat: implement admin reporting endpoints (#43) ([9568385](https://github.com/seifmaazouz/order-processing-system/commit/9568385))
* feat: implement complete books API with CRUD and search (#33) ([76df09d](https://github.com/seifmaazouz/order-processing-system/commit/76df09d))
* feat: create init script (#22) ([347aa22](https://github.com/seifmaazouz/order-processing-system/commit/347aa22))
* feat: docker setup for database (#23) ([1ea43e4](https://github.com/seifmaazouz/order-processing-system/commit/1ea43e4))
* feat: define ERD, relational schema, and docker setup (#9) ([34081ea](https://github.com/seifmaazouz/order-processing-system/commit/34081ea))
* feat: system report queries and sample data (#38) ([8bee573](https://github.com/seifmaazouz/order-processing-system/commit/8bee573))
* feat: update schema and split order table (#46) ([94d29f5](https://github.com/seifmaazouz/order-processing-system/commit/94d29f5))

## Bug Fixes
* fix: update VITE_API_BASE_URL and PR title trigger ([20f3756](https://github.com/seifmaazouz/order-processing-system/commit/20f3756))
* fix: include title in customer order item creation (#79) ([88cdf44](https://github.com/seifmaazouz/order-processing-system/commit/88cdf44))
* fix: shopping cart stock handling and responses (#76) ([bf10552](https://github.com/seifmaazouz/order-processing-system/commit/bf10552))
* fix: release pipeline version detection and docker build (#66) ([7f49acf](https://github.com/seifmaazouz/order-processing-system/commit/7f49acf))
* fix: cart stock display and increment handling (#80) ([801eb1a](https://github.com/seifmaazouz/order-processing-system/commit/801eb1a))
* fix: authentication and add-to-cart logic (#56) ([29d76e5](https://github.com/seifmaazouz/order-processing-system/commit/29d76e5))
* fix: SQL queries after testing (#41) ([a93e666](https://github.com/seifmaazouz/order-processing-system/commit/a93e666))
* fix: query attributes matching schema (#39) ([e2fcb1b](https://github.com/seifmaazouz/order-processing-system/commit/e2fcb1b))
* fix: init.sql syntax and docker compose (#28) ([aacea0f](https://github.com/seifmaazouz/order-processing-system/commit/aacea0f))
* fix: triggers with hardcoded values (#59) ([0e3b901](https://github.com/seifmaazouz/order-processing-system/commit/0e3b901))

## Refactoring
* refactor: introduce CartItemReadModel and mappings (#78) ([9e90d03](https://github.com/seifmaazouz/order-processing-system/commit/9e90d03))
* refactor: simplify customer order item retrieval (#77) ([df13dbe](https://github.com/seifmaazouz/order-processing-system/commit/df13dbe))
* refactor: clean architecture and infrastructure wiring (#13) ([db77f5d](https://github.com/seifmaazouz/order-processing-system/commit/db77f5d))
* refactor: unified book search field (#42) ([93d951d](https://github.com/seifmaazouz/order-processing-system/commit/93d951d))
* refactor: cart item update API (#57) ([b1c8e98](https://github.com/seifmaazouz/order-processing-system/commit/b1c8e98))
* refactor: cart logic and checkout improvements (#61) ([bda7e65](https://github.com/seifmaazouz/order-processing-system/commit/bda7e65))
* refactor: admin order management and code cleanup (#55) ([17908bf](https://github.com/seifmaazouz/order-processing-system/commit/17908bf))
* refactor: IDbConnectionFactory async connection handling (#20) ([941f969](https://github.com/seifmaazouz/order-processing-system/commit/941f969))

## Backend / Domain / Data
* feat: add domain entities (Book, Category, Publisher) ([0b4384e](https://github.com/seifmaazouz/order-processing-system/commit/0b4384e))
* feat: add user entity (#7) ([74f9a79](https://github.com/seifmaazouz/order-processing-system/commit/74f9a79))
* feat: create repositories (#16) ([034a3f1](https://github.com/seifmaazouz/order-processing-system/commit/034a3f1))
* feat: add domain repositories (#29) ([e3f7a0b](https://github.com/seifmaazouz/order-processing-system/commit/e3f7a0b))
* feat: add services layer (#32) ([18fd5f9](https://github.com/seifmaazouz/order-processing-system/commit/18fd5f9))
* feat: modify user, credit card, and order entities ([3d72550](https://github.com/seifmaazouz/order-processing-system/commit/3d72550))
* feat: modify entities for credit card and order items (#11) ([9f06369](https://github.com/seifmaazouz/order-processing-system/commit/9f06369))

## Security
* feat: add JWT implementation (#40) ([e1336b0](https://github.com/seifmaazouz/order-processing-system/commit/e1336b0))
* fix: register admin endpoints (#45) ([ff974e4](https://github.com/seifmaazouz/order-processing-system/commit/ff974e4))
* fix: enforce role assignment and user endpoints (#52) ([16cef1c](https://github.com/seifmaazouz/order-processing-system/commit/16cef1c))

## DevOps / CI / Setup
* feat: add backend service to docker-compose (#35) ([5162cff](https://github.com/seifmaazouz/order-processing-system/commit/5162cff))
* feat: add CORS policy (#36) ([d617552](https://github.com/seifmaazouz/order-processing-system/commit/d617552))
* chore: add production Dockerfile (#74) ([8ed935b](https://github.com/seifmaazouz/order-processing-system/commit/8ed935b))
* chore: remove build artifacts (#73, #75) ([030c336](https://github.com/seifmaazouz/order-processing-system/commit/030c336), [4ef859c](https://github.com/seifmaazouz/order-processing-system/commit/4ef859c))
* chore: fix semantic versioning (#70) ([484a37e](https://github.com/seifmaazouz/order-processing-system/commit/484a37e))

## Documentation
* docs: add API error handling and ServiceResult pattern ([8bd50fb](https://github.com/seifmaazouz/order-processing-system/commit/8bd50fb))
* docs: add backend setup guide ([09b9212](https://github.com/seifmaazouz/order-processing-system/commit/09b9212))
* docs: update README and setup guide (#6) ([4e26f20](https://github.com/seifmaazouz/order-processing-system/commit/4e26f20))
* docs: finalize semantic-release setup (#64) ([72df539](https://github.com/seifmaazouz/order-processing-system/commit/72df539))
* docs: add API documentation (#25) ([938a7c8](https://github.com/seifmaazouz/order-processing-system/commit/938a7c8))

## Chores
* chore: add CODEOWNERS and workflow (#1) ([c7cafe8](https://github.com/seifmaazouz/order-processing-system/commit/c7cafe8))
* chore: enforce lowercase branch naming (#8) ([d45a5e0](https://github.com/seifmaazouz/order-processing-system/commit/d45a5e0))
* chore: improve branch ruleset handling ([bf83679](https://github.com/seifmaazouz/order-processing-system/commit/bf83679))
* chore: fix file structure (#18) ([f76b42d](https://github.com/seifmaazouz/order-processing-system/commit/f76b42d))

## BREAKING CHANGES
* Simplified to `main`/`dev` branches only; removed release/hotfix branches.