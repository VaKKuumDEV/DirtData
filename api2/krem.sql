-- phpMyAdmin SQL Dump
-- version 5.0.4deb2
-- https://www.phpmyadmin.net/
--
-- Хост: localhost:3306
-- Время создания: Авг 01 2022 г., 15:18
-- Версия сервера: 10.5.15-MariaDB-0+deb11u1
-- Версия PHP: 7.4.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `krem`
--

-- --------------------------------------------------------

--
-- Структура таблицы `attachments`
--

CREATE TABLE `attachments` (
  `attachment_id` int(11) NOT NULL,
  `attachment_owner` int(11) DEFAULT NULL,
  `attachment_link` text NOT NULL,
  `attachment_date` datetime NOT NULL DEFAULT current_timestamp(),
  `attachment_type` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `categories`
--

CREATE TABLE `categories` (
  `category_id` int(11) NOT NULL,
  `category_name` varchar(255) NOT NULL,
  `category_parent` int(11) DEFAULT NULL,
  `category_status` int(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `categories`
--

INSERT INTO `categories` (`category_id`, `category_name`, `category_parent`, `category_status`) VALUES
(1, 'Анализ воды', NULL, 1),
(2, 'Анализ почвы', NULL, 1),
(3, 'Природные воды', 1, 1),
(4, 'Сточные воды', 1, 1),
(5, 'Почвы', 2, 1),
(6, 'Грунты', 2, 1),
(7, 'Донные отложения', 2, 1),
(8, 'Осадки сточных вод', 2, 1),
(9, 'Аммоний-ион', 4, 1),
(10, 'Биохимическое потребление кислорода (БПКп)', 4, 1),
(11, 'Взвешенные вещества', 4, 1),
(12, 'Водородный показатель (рН)', 4, 1),
(13, 'Кальций', 4, 1),
(14, 'Магний', 4, 1),
(15, 'Нефтепродукты', 4, 1),
(16, 'Нитрат-ион', 4, 1),
(17, 'Сухой остаток', 4, 1),
(18, 'ХПК', 4, 1),
(19, 'Железо', 4, 1),
(20, 'Кадмий', 4, 1),
(21, 'Калий', 4, 1),
(22, 'Кобальт', 4, 1),
(23, 'Марганец', 4, 1),
(24, 'Медь', 4, 1),
(25, 'Молибден', 4, 1),
(26, 'Натрий', 4, 1),
(27, 'Никель', 4, 1),
(28, 'Свинец', 4, 1),
(29, 'Хром', 4, 1),
(30, 'Цинк', 4, 1),
(31, 'Аммоний-ион', 3, 1),
(32, 'Биохимическое потребление кислорода (БПКп)', 3, 1),
(33, 'Взвешенные вещества', 3, 1),
(34, 'Водородный показатель (рН)', 3, 1),
(35, 'Кальций', 3, 1),
(36, 'Магний', 3, 1),
(37, 'Нефтепродукты', 3, 1),
(38, 'Нитрат-ион', 3, 1),
(39, 'Сухой остаток', 3, 1),
(40, 'ХПК', 3, 1),
(41, 'Жесткость', 3, 1),
(42, 'Мутность', 3, 1),
(43, 'Цветность', 3, 1),
(44, 'Железо', 3, 1),
(45, 'Кадмий', 3, 1),
(46, 'Калий', 3, 1),
(47, 'Кобальт', 3, 1),
(48, 'Марганец', 3, 1),
(49, 'Медь', 3, 1),
(50, 'Молибден', 3, 1),
(51, 'Натрий', 3, 1),
(52, 'Никель', 3, 1),
(53, 'Свинец', 3, 1),
(54, 'Хром', 3, 1),
(55, 'Цинк', 3, 1),
(56, 'Азот нитратов', 5, 1),
(57, 'Азот нитритный', 5, 1),
(58, 'Азот общий', 5, 1),
(59, 'Ионы аммония', 5, 1),
(60, 'Бикарбонат-ион', 5, 1),
(61, 'Карбонат-ион', 5, 1),
(62, 'Влага (влажность)', 5, 1),
(63, 'Водородный показатель (рН) водной вытяжки', 5, 1),
(64, 'Плотный остаток водной вытяжки', 5, 1),
(65, 'Водородный показатель (рН)', 5, 1),
(66, 'Гексахлорциклогексан (α-, ᵧ-)', 5, 1),
(67, '4,4-дихлордифенил-трихлорэтан (ДДТ)', 5, 1),
(68, 'Зола (зольность)', 5, 1),
(69, 'Потери массы при прокаливании', 5, 1),
(70, 'Кальций', 5, 1),
(71, 'Магний', 5, 1),
(72, 'Нитрат-ион', 5, 1),
(73, 'Органическое вещество', 5, 1),
(74, 'Нефтепродукты', 5, 1),
(75, 'Сульфат-ион', 5, 1),
(76, 'Хлорид-ион', 5, 1),
(77, 'Обменный аммоний', 5, 1),
(78, 'Гидролитическая кислотность', 5, 1),
(79, 'Обменная кислотность', 5, 1),
(80, 'Обменный натрий', 5, 1),
(81, 'Подвижный калий', 5, 1),
(82, 'Подвижный фосфор', 5, 1),
(83, 'Железо', 5, 1),
(84, 'Кадмий', 5, 1),
(85, 'Кобальт', 5, 1),
(86, 'Марганец', 5, 1),
(87, 'Медь', 5, 1),
(88, 'Молибден', 5, 1),
(89, 'Никель', 5, 1),
(90, 'Свинец', 5, 1),
(91, 'Хром', 5, 1),
(92, 'Цинк', 5, 1),
(93, 'Калий', 5, 1),
(94, 'Натрий', 5, 1),
(95, 'Азот общий', 6, 1),
(96, 'Ионы аммония', 6, 1),
(97, 'Влага (влажность)', 6, 1),
(98, 'Водородный показатель (рН)', 6, 1),
(99, 'Зола (зольность)', 6, 1),
(100, 'Потери массы при прокаливании', 6, 1),
(101, 'Кальций', 6, 1),
(102, 'Магний', 6, 1),
(103, 'Фосфор общий', 6, 1),
(104, 'Хлорид-ион', 6, 1),
(105, 'Азот нитратов', 7, 1),
(106, 'Азот нитритный', 7, 1),
(107, 'Ионы аммония', 7, 1),
(108, 'Влага (влажность)', 7, 1),
(109, 'Водородный показатель (рН)', 7, 1),
(110, 'Гексахлорциклогексан (α-, ᵧ-)', 7, 1),
(111, '4,4-дихлордифенил-трихлорэтан (ДДТ)', 7, 1),
(112, 'Зола (зольность)', 7, 1),
(113, 'Потери массы при прокаливании', 7, 1),
(114, 'Кальций', 7, 1),
(115, 'Магний', 7, 1),
(116, 'Нефтепродукты', 7, 1),
(117, 'Сульфат-ион', 7, 1),
(118, 'Хлорид-ион', 7, 1),
(119, 'Железо', 7, 1),
(120, 'Кадмий', 7, 1),
(121, 'Калий', 7, 1),
(122, 'Кальций', 7, 1),
(123, 'Кобальт', 7, 1),
(124, 'Магний', 7, 1),
(125, 'Марганец', 7, 1),
(126, 'Медь', 7, 1),
(127, 'Натрий', 7, 1),
(128, 'Никель', 7, 1),
(129, 'Свинец', 7, 1),
(130, 'Хром', 7, 1),
(131, 'Цинк', 7, 1),
(132, 'Азот общий', 8, 1),
(133, 'Ионы аммония', 8, 1),
(134, 'Влага (влажность)', 8, 1),
(135, 'Водородный показатель (рН)', 8, 1),
(136, 'Зола (зольность)', 8, 1),
(137, 'Потери массы при прокаливании', 8, 1),
(138, 'Кальций', 8, 1),
(139, 'Магний', 8, 1),
(140, 'Нефтепродукты', 8, 1),
(141, 'Фосфор общий', 8, 1),
(142, 'Хлорид-ион', 8, 1),
(143, 'Железо', 8, 1),
(144, 'Кадмий', 8, 1),
(145, 'Калий', 8, 1),
(146, 'Кальций', 8, 1),
(147, 'Кобальт', 8, 1),
(148, 'Магний', 8, 1),
(149, 'Марганец', 8, 1),
(150, 'Медь', 8, 1),
(151, 'Натрий', 8, 1),
(152, 'Никель', 8, 1),
(153, 'Свинец', 8, 1),
(154, 'Хром', 8, 1),
(155, 'Цинк', 8, 1);

-- --------------------------------------------------------

--
-- Структура таблицы `dirtdata`
--

CREATE TABLE `dirtdata` (
  `data_id` int(11) NOT NULL,
  `data_sender` int(11) DEFAULT NULL,
  `data_date` datetime NOT NULL DEFAULT current_timestamp(),
  `data_content` longtext NOT NULL,
  `data_longitude` double(100,6) NOT NULL,
  `data_latitude` double(100,6) NOT NULL,
  `data_attachments` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `servers`
--

CREATE TABLE `servers` (
  `server_id` int(11) NOT NULL,
  `server_name` varchar(255) NOT NULL,
  `server_address` varchar(255) NOT NULL,
  `server_status` int(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `users`
--

CREATE TABLE `users` (
  `user_id` int(11) NOT NULL,
  `user_login` varchar(255) NOT NULL,
  `user_password` varchar(255) NOT NULL,
  `user_reg_date` datetime NOT NULL DEFAULT current_timestamp(),
  `user_hash` varchar(255) NOT NULL,
  `user_status` int(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `attachments`
--
ALTER TABLE `attachments`
  ADD PRIMARY KEY (`attachment_id`),
  ADD KEY `attachments_ibfk_1` (`attachment_owner`);

--
-- Индексы таблицы `categories`
--
ALTER TABLE `categories`
  ADD PRIMARY KEY (`category_id`),
  ADD KEY `category_parent` (`category_parent`);

--
-- Индексы таблицы `dirtdata`
--
ALTER TABLE `dirtdata`
  ADD PRIMARY KEY (`data_id`),
  ADD KEY `data_sender` (`data_sender`);

--
-- Индексы таблицы `servers`
--
ALTER TABLE `servers`
  ADD PRIMARY KEY (`server_id`);

--
-- Индексы таблицы `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`user_id`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `attachments`
--
ALTER TABLE `attachments`
  MODIFY `attachment_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `categories`
--
ALTER TABLE `categories`
  MODIFY `category_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=156;

--
-- AUTO_INCREMENT для таблицы `dirtdata`
--
ALTER TABLE `dirtdata`
  MODIFY `data_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `servers`
--
ALTER TABLE `servers`
  MODIFY `server_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `users`
--
ALTER TABLE `users`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `attachments`
--
ALTER TABLE `attachments`
  ADD CONSTRAINT `attachments_ibfk_1` FOREIGN KEY (`attachment_owner`) REFERENCES `users` (`user_id`) ON DELETE SET NULL ON UPDATE SET NULL;

--
-- Ограничения внешнего ключа таблицы `categories`
--
ALTER TABLE `categories`
  ADD CONSTRAINT `categories_ibfk_1` FOREIGN KEY (`category_parent`) REFERENCES `categories` (`category_id`) ON DELETE SET NULL ON UPDATE SET NULL;

--
-- Ограничения внешнего ключа таблицы `dirtdata`
--
ALTER TABLE `dirtdata`
  ADD CONSTRAINT `dirtdata_ibfk_1` FOREIGN KEY (`data_sender`) REFERENCES `users` (`user_id`) ON DELETE SET NULL ON UPDATE SET NULL;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
